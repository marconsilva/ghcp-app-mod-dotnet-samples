# ============================================
# ContosoUniversity - Azure Container Apps Deployment (PowerShell)
# ============================================
# This script deploys ContosoUniversity to Azure Container Apps
# with all required Azure resources
# ============================================

$ErrorActionPreference = "Stop"

# Configuration variables
$RESOURCE_GROUP = "contoso-rg"
$LOCATION = "eastus"
$ACR_NAME = "contosoacr$(Get-Random -Maximum 99999)"
$ENVIRONMENT_NAME = "contoso-env"
$CONTAINER_APP_NAME = "contoso-web"
$SQL_SERVER_NAME = "contoso-sqlserver-$(Get-Random -Maximum 99999)"
$SQL_DB_NAME = "ContosoUniversity"
$SQL_ADMIN_USER = "sqladmin"
$SQL_ADMIN_PASSWORD = "$(New-Guid)Aa1!".Substring(0, 20) + "Aa1!"
$STORAGE_ACCOUNT_NAME = "contosostorage$(Get-Random -Maximum 99999)"
$LOG_WORKSPACE_NAME = "contoso-logs"

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "ContosoUniversity - Azure Container Apps" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Check if Azure CLI is installed
try {
    az --version | Out-Null
}
catch {
    Write-Host "Error: Azure CLI is not installed" -ForegroundColor Red
    Write-Host "Please install from: https://aka.ms/installazurecliwindows"
    exit 1
}

# Check if logged in
Write-Host "Checking Azure login status..." -ForegroundColor Yellow
try {
    $subscription = az account show --query name -o tsv
    Write-Host "? Logged in to subscription: $subscription" -ForegroundColor Green
}
catch {
    Write-Host "Not logged in. Please login to Azure..." -ForegroundColor Yellow
    az login
    $subscription = az account show --query name -o tsv
}
Write-Host ""

# Save credentials
$SQL_ADMIN_PASSWORD | Out-File -FilePath ".deployment-credentials.txt" -NoNewline
Write-Host "? SQL password saved to .deployment-credentials.txt (DO NOT COMMIT)" -ForegroundColor Yellow
Write-Host ""

# Step 1: Create Resource Group
Write-Host "Step 1/10: Creating resource group..." -ForegroundColor Yellow
az group create `
  --name $RESOURCE_GROUP `
  --location $LOCATION `
  --output none
Write-Host "? Resource group created: $RESOURCE_GROUP" -ForegroundColor Green
Write-Host ""

# Step 2: Create Container Registry
Write-Host "Step 2/10: Creating Azure Container Registry..." -ForegroundColor Yellow
az acr create `
  --resource-group $RESOURCE_GROUP `
  --name $ACR_NAME `
  --sku Standard `
  --admin-enabled true `
  --output none
Write-Host "? Container Registry created: $ACR_NAME" -ForegroundColor Green
Write-Host ""

# Step 3: Create Log Analytics Workspace
Write-Host "Step 3/10: Creating Log Analytics Workspace..." -ForegroundColor Yellow
az monitor log-analytics workspace create `
  --resource-group $RESOURCE_GROUP `
  --workspace-name $LOG_WORKSPACE_NAME `
  --location $LOCATION `
  --output none
Write-Host "? Log Analytics Workspace created" -ForegroundColor Green
Write-Host ""

# Get workspace credentials
Write-Host "Getting Log Analytics credentials..." -ForegroundColor Yellow
$LOG_ANALYTICS_WORKSPACE_CLIENT_ID = az monitor log-analytics workspace show `
  --resource-group $RESOURCE_GROUP `
  --workspace-name $LOG_WORKSPACE_NAME `
  --query customerId `
  --output tsv

$LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET = az monitor log-analytics workspace get-shared-keys `
  --resource-group $RESOURCE_GROUP `
  --workspace-name $LOG_WORKSPACE_NAME `
  --query primarySharedKey `
  --output tsv
Write-Host "? Workspace credentials retrieved" -ForegroundColor Green
Write-Host ""

# Step 4: Create Container Apps Environment
Write-Host "Step 4/10: Creating Container Apps Environment..." -ForegroundColor Yellow
az containerapp env create `
  --name $ENVIRONMENT_NAME `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION `
  --logs-workspace-id $LOG_ANALYTICS_WORKSPACE_CLIENT_ID `
  --logs-workspace-key $LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET `
  --output none
Write-Host "? Container Apps Environment created: $ENVIRONMENT_NAME" -ForegroundColor Green
Write-Host ""

# Step 5: Create Azure SQL Server and Database
Write-Host "Step 5/10: Creating Azure SQL Server and Database..." -ForegroundColor Yellow
az sql server create `
  --name $SQL_SERVER_NAME `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION `
  --admin-user $SQL_ADMIN_USER `
  --admin-password $SQL_ADMIN_PASSWORD `
  --output none
Write-Host "? SQL Server created: $SQL_SERVER_NAME" -ForegroundColor Green

az sql db create `
  --resource-group $RESOURCE_GROUP `
  --server $SQL_SERVER_NAME `
  --name $SQL_DB_NAME `
  --service-objective S2 `
  --backup-storage-redundancy Local `
  --output none
Write-Host "? SQL Database created: $SQL_DB_NAME (S2 tier)" -ForegroundColor Green

# Configure firewall
az sql server firewall-rule create `
  --resource-group $RESOURCE_GROUP `
  --server $SQL_SERVER_NAME `
  --name AllowAzureServices `
  --start-ip-address 0.0.0.0 `
  --end-ip-address 0.0.0.0 `
  --output none
Write-Host "? Firewall configured to allow Azure services" -ForegroundColor Green
Write-Host ""

# Step 6: Create Storage Account
Write-Host "Step 6/10: Creating Azure Storage Account..." -ForegroundColor Yellow
az storage account create `
  --name $STORAGE_ACCOUNT_NAME `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION `
  --sku Standard_LRS `
  --kind StorageV2 `
  --output none
Write-Host "? Storage Account created: $STORAGE_ACCOUNT_NAME" -ForegroundColor Green

# Get storage account key
$STORAGE_KEY = az storage account keys list `
  --resource-group $RESOURCE_GROUP `
  --account-name $STORAGE_ACCOUNT_NAME `
  --query "[0].value" `
  --output tsv

# Create file share
az storage share create `
  --name uploads `
  --account-name $STORAGE_ACCOUNT_NAME `
  --account-key $STORAGE_KEY `
  --quota 10 `
  --output none
Write-Host "? File share created: uploads (10 GB)" -ForegroundColor Green
Write-Host ""

# Step 7: Configure storage in Container Apps Environment
Write-Host "Step 7/10: Configuring storage mount..." -ForegroundColor Yellow
az containerapp env storage set `
  --name $ENVIRONMENT_NAME `
  --resource-group $RESOURCE_GROUP `
  --storage-name uploads `
  --azure-file-account-name $STORAGE_ACCOUNT_NAME `
  --azure-file-account-key $STORAGE_KEY `
  --azure-file-share-name uploads `
  --access-mode ReadWrite `
  --output none
Write-Host "? Storage mount configured" -ForegroundColor Green
Write-Host ""

# Step 8: Build and push Docker image
Write-Host "Step 8/10: Building and pushing Docker image..." -ForegroundColor Yellow
Write-Host "This may take 3-5 minutes..." -ForegroundColor Yellow

# Navigate to project root
Set-Location (Split-Path $PSScriptRoot -Parent)

az acr build `
  --registry $ACR_NAME `
  --image contosouniversity:latest `
  --image contosouniversity:v1.0 `
  --file Dockerfile `
  . `
  --output table

Write-Host "? Docker image built and pushed" -ForegroundColor Green
Write-Host ""

# Step 9: Deploy Container App
Write-Host "Step 9/10: Deploying Container App..." -ForegroundColor Yellow

# Build connection string
$SQL_CONNECTION_STRING = "Server=tcp:${SQL_SERVER_NAME}.database.windows.net,1433;Database=${SQL_DB_NAME};User ID=${SQL_ADMIN_USER};Password=${SQL_ADMIN_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# Deploy container app
az containerapp create `
  --name $CONTAINER_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --environment $ENVIRONMENT_NAME `
  --image "${ACR_NAME}.azurecr.io/contosouniversity:latest" `
  --target-port 8080 `
  --ingress 'external' `
  --registry-server "${ACR_NAME}.azurecr.io" `
  --registry-identity system `
  --cpu 1.0 `
  --memory 2.0Gi `
  --min-replicas 2 `
  --max-replicas 30 `
  --secrets "sql-connection-string=$SQL_CONNECTION_STRING" `
  --env-vars "ASPNETCORE_ENVIRONMENT=Production" "ConnectionStrings__DefaultConnection=secretref:sql-connection-string" "NotificationQueuePath=.\\Private`$\\ContosoUniversityNotifications" `
  --output none

Write-Host "? Container App deployed" -ForegroundColor Green
Write-Host ""

# Step 10: Configure scaling rules
Write-Host "Step 10/10: Configuring auto-scaling..." -ForegroundColor Yellow
az containerapp update `
  --name $CONTAINER_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --scale-rule-name http-scaling `
  --scale-rule-type http `
  --scale-rule-http-concurrency 100 `
  --output none
Write-Host "? Auto-scaling configured" -ForegroundColor Green
Write-Host ""

# Enable managed identity for ACR pull
Write-Host "Configuring managed identity..." -ForegroundColor Yellow
$PRINCIPAL_ID = az containerapp identity show `
  --name $CONTAINER_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --query principalId `
  --output tsv

$ACR_ID = az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query id --output tsv

az role assignment create `
  --assignee $PRINCIPAL_ID `
  --role AcrPull `
  --scope $ACR_ID `
  --output none
Write-Host "? Managed identity configured for ACR pull" -ForegroundColor Green
Write-Host ""

# Get application URL
$APP_URL = az containerapp show `
  --name $CONTAINER_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --query properties.configuration.ingress.fqdn `
  --output tsv

# Wait for app to be ready
Write-Host "Waiting for application to be ready..." -ForegroundColor Yellow
$ready = $false
for ($i = 1; $i -le 30; $i++) {
    try {
        $response = Invoke-WebRequest -Uri "https://$APP_URL/health" -UseBasicParsing -TimeoutSec 3
        if ($response.StatusCode -eq 200) {
            Write-Host "? Application is healthy" -ForegroundColor Green
            $ready = $true
            break
        }
    }
    catch {
        Write-Host "." -NoNewline
        Start-Sleep -Seconds 5
    }
}

if (-not $ready) {
    Write-Host ""
    Write-Host "Warning: Health check timeout. Check logs." -ForegroundColor Yellow
}
Write-Host ""

# ============================================
# Deployment Summary
# ============================================
Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "? Deployment Complete!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Write-Host "Application URL:" -ForegroundColor Cyan
Write-Host "  https://$APP_URL" -ForegroundColor White
Write-Host ""
Write-Host "Health Check:" -ForegroundColor Cyan
Write-Host "  https://$APP_URL/health" -ForegroundColor White
Write-Host ""
Write-Host "Azure Portal:" -ForegroundColor Cyan
$subscriptionId = az account show --query id -o tsv
Write-Host "  https://portal.azure.com/#@/resource/subscriptions/$subscriptionId/resourceGroups/$RESOURCE_GROUP/overview" -ForegroundColor White
Write-Host ""
Write-Host "Resources Created:" -ForegroundColor Cyan
Write-Host "  • Resource Group:     $RESOURCE_GROUP" -ForegroundColor White
Write-Host "  • Container Registry: $ACR_NAME.azurecr.io" -ForegroundColor White
Write-Host "  • Container App:      $CONTAINER_APP_NAME" -ForegroundColor White
Write-Host "  • SQL Server:         $SQL_SERVER_NAME.database.windows.net" -ForegroundColor White
Write-Host "  • SQL Database:       $SQL_DB_NAME (S2 tier)" -ForegroundColor White
Write-Host "  • Storage Account:    $STORAGE_ACCOUNT_NAME" -ForegroundColor White
Write-Host "  • Log Analytics:      $LOG_WORKSPACE_NAME" -ForegroundColor White
Write-Host ""
Write-Host "Credentials (SECURE THESE):" -ForegroundColor Cyan
Write-Host "  SQL Admin User:       $SQL_ADMIN_USER" -ForegroundColor White
Write-Host "  SQL Admin Password:   (saved in .deployment-credentials.txt)" -ForegroundColor Yellow
Write-Host ""
Write-Host "Scaling Configuration:" -ForegroundColor Cyan
Write-Host "  Min Replicas:         2 (High Availability)" -ForegroundColor White
Write-Host "  Max Replicas:         30 (Auto-scaling)" -ForegroundColor White
Write-Host "  Scale Trigger:        100 concurrent HTTP requests" -ForegroundColor White
Write-Host ""
Write-Host "Estimated Monthly Cost:" -ForegroundColor Cyan
Write-Host "  Container Apps:       ~`$100-200 (variable)" -ForegroundColor White
Write-Host "  SQL Database (S2):    ~`$150" -ForegroundColor White
Write-Host "  Storage:              ~`$20" -ForegroundColor White
Write-Host "  Container Registry:   ~`$20" -ForegroundColor White
Write-Host "  Log Analytics:        ~`$30" -ForegroundColor White
Write-Host "  Total: ~`$320-420/month" -ForegroundColor Green
Write-Host ""
Write-Host "Useful Commands:" -ForegroundColor Cyan
Write-Host "  View logs:" -ForegroundColor White
Write-Host "    az containerapp logs tail --name $CONTAINER_APP_NAME --resource-group $RESOURCE_GROUP --follow" -ForegroundColor Gray
Write-Host ""
Write-Host "  Scale manually:" -ForegroundColor White
Write-Host "    az containerapp update --name $CONTAINER_APP_NAME --resource-group $RESOURCE_GROUP --min-replicas 3 --max-replicas 20" -ForegroundColor Gray
Write-Host ""
Write-Host "  Update image:" -ForegroundColor White
Write-Host "    az containerapp update --name $CONTAINER_APP_NAME --resource-group $RESOURCE_GROUP --image $ACR_NAME.azurecr.io/contosouniversity:v1.1" -ForegroundColor Gray
Write-Host ""
Write-Host "?? Deployment successful!" -ForegroundColor Green
Write-Host "Visit your application at: https://$APP_URL" -ForegroundColor Green
Write-Host ""

# Open browser
Start-Process "https://$APP_URL"
