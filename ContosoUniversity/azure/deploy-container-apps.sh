# Azure Container Apps deployment configuration for ContosoUniversity

# Resource Group
az group create \
  --name contoso-rg \
  --location eastus

# Container Registry
az acr create \
  --resource-group contoso-rg \
  --name contosoacr \
  --sku Standard

# Log Analytics Workspace
az monitor log-analytics workspace create \
  --resource-group contoso-rg \
  --workspace-name contoso-logs

# Get workspace credentials
LOG_ANALYTICS_WORKSPACE_CLIENT_ID=$(az monitor log-analytics workspace show \
  --resource-group contoso-rg \
  --workspace-name contoso-logs \
  --query customerId \
  --output tsv)

LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET=$(az monitor log-analytics workspace get-shared-keys \
  --resource-group contoso-rg \
  --workspace-name contoso-logs \
  --query primarySharedKey \
  --output tsv)

# Container Apps Environment
az containerapp env create \
  --name contoso-env \
  --resource-group contoso-rg \
  --location eastus \
  --logs-workspace-id $LOG_ANALYTICS_WORKSPACE_CLIENT_ID \
  --logs-workspace-key $LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET

# Azure SQL Database
az sql server create \
  --name contoso-sqlserver \
  --resource-group contoso-rg \
  --location eastus \
  --admin-user sqladmin \
  --admin-password 'YourStrong!Passw0rd'

az sql db create \
  --resource-group contoso-rg \
  --server contoso-sqlserver \
  --name ContosoUniversity \
  --service-objective S0

# Configure firewall
az sql server firewall-rule create \
  --resource-group contoso-rg \
  --server contoso-sqlserver \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Azure Storage Account for file uploads
az storage account create \
  --name contosostorage \
  --resource-group contoso-rg \
  --location eastus \
  --sku Standard_LRS

# Create file share for uploads
az storage share create \
  --name uploads \
  --account-name contosostorage

# Build and push Docker image
az acr build \
  --registry contosoacr \
  --image contosouniversity:latest \
  --file Dockerfile \
  .

# Deploy Container App
az containerapp create \
  --name contoso-web \
  --resource-group contoso-rg \
  --environment contoso-env \
  --image contosoacr.azurecr.io/contosouniversity:latest \
  --target-port 8080 \
  --ingress 'external' \
  --registry-server contosoacr.azurecr.io \
  --cpu 1.0 \
  --memory 2.0Gi \
  --min-replicas 1 \
  --max-replicas 10 \
  --env-vars \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection='Server=tcp:contoso-sqlserver.database.windows.net,1433;Database=ContosoUniversity;User ID=sqladmin;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;' \
    NotificationQueuePath='.\\Private$\\ContosoUniversityNotifications'

# Enable auto-scaling
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --scale-rule-name http-rule \
  --scale-rule-type http \
  --scale-rule-http-concurrency 100

echo "Deployment complete!"
echo "Application URL:"
az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query properties.configuration.ingress.fqdn \
  --output tsv
