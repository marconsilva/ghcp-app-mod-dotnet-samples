#!/bin/bash

# ============================================
# ContosoUniversity - Azure Container Apps Deployment
# ============================================
# This script deploys ContosoUniversity to Azure Container Apps
# with all required Azure resources
# ============================================

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration variables
RESOURCE_GROUP="contoso-rg"
LOCATION="eastus"
ACR_NAME="contosoacr$(date +%s | tail -c 5)"  # Unique name
ENVIRONMENT_NAME="contoso-env"
CONTAINER_APP_NAME="contoso-web"
SQL_SERVER_NAME="contoso-sqlserver-$(date +%s | tail -c 5)"
SQL_DB_NAME="ContosoUniversity"
SQL_ADMIN_USER="sqladmin"
SQL_ADMIN_PASSWORD="$(openssl rand -base64 24 | tr -d "=+/" | cut -c1-20)Aa1!"
STORAGE_ACCOUNT_NAME="contosostorage$(date +%s | tail -c 5)"
LOG_WORKSPACE_NAME="contoso-logs"

echo -e "${BLUE}============================================${NC}"
echo -e "${BLUE}ContosoUniversity - Azure Container Apps${NC}"
echo -e "${BLUE}============================================${NC}"
echo ""

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo -e "${RED}Error: Azure CLI is not installed${NC}"
    echo "Please install from: https://aka.ms/installazurecliwindows"
    exit 1
fi

# Check if logged in
echo -e "${YELLOW}Checking Azure login status...${NC}"
if ! az account show &> /dev/null; then
    echo -e "${YELLOW}Not logged in. Please login to Azure...${NC}"
    az login
fi

SUBSCRIPTION_NAME=$(az account show --query name -o tsv)
echo -e "${GREEN}? Logged in to subscription: ${SUBSCRIPTION_NAME}${NC}"
echo ""

# Save credentials for later use
echo "$SQL_ADMIN_PASSWORD" > .deployment-credentials.txt
echo -e "${YELLOW}? SQL password saved to .deployment-credentials.txt (DO NOT COMMIT)${NC}"
echo ""

# Step 1: Create Resource Group
echo -e "${YELLOW}Step 1/10: Creating resource group...${NC}"
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION \
  --output none
echo -e "${GREEN}? Resource group created: $RESOURCE_GROUP${NC}"
echo ""

# Step 2: Create Container Registry
echo -e "${YELLOW}Step 2/10: Creating Azure Container Registry...${NC}"
az acr create \
  --resource-group $RESOURCE_GROUP \
  --name $ACR_NAME \
  --sku Standard \
  --admin-enabled true \
  --output none
echo -e "${GREEN}? Container Registry created: $ACR_NAME${NC}"
echo ""

# Step 3: Create Log Analytics Workspace
echo -e "${YELLOW}Step 3/10: Creating Log Analytics Workspace...${NC}"
az monitor log-analytics workspace create \
  --resource-group $RESOURCE_GROUP \
  --workspace-name $LOG_WORKSPACE_NAME \
  --location $LOCATION \
  --output none
  
echo -e "${GREEN}? Log Analytics Workspace created${NC}"
echo ""

# Get workspace credentials
echo -e "${YELLOW}Getting Log Analytics credentials...${NC}"
LOG_ANALYTICS_WORKSPACE_CLIENT_ID=$(az monitor log-analytics workspace show \
  --resource-group $RESOURCE_GROUP \
  --workspace-name $LOG_WORKSPACE_NAME \
  --query customerId \
  --output tsv)

LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET=$(az monitor log-analytics workspace get-shared-keys \
  --resource-group $RESOURCE_GROUP \
  --workspace-name $LOG_WORKSPACE_NAME \
  --query primarySharedKey \
  --output tsv)
echo -e "${GREEN}? Workspace credentials retrieved${NC}"
echo ""

# Step 4: Create Container Apps Environment
echo -e "${YELLOW}Step 4/10: Creating Container Apps Environment...${NC}"
az containerapp env create \
  --name $ENVIRONMENT_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --logs-workspace-id $LOG_ANALYTICS_WORKSPACE_CLIENT_ID \
  --logs-workspace-key $LOG_ANALYTICS_WORKSPACE_CLIENT_SECRET \
  --output none
echo -e "${GREEN}? Container Apps Environment created: $ENVIRONMENT_NAME${NC}"
echo ""

# Step 5: Create Azure SQL Server and Database
echo -e "${YELLOW}Step 5/10: Creating Azure SQL Server and Database...${NC}"
az sql server create \
  --name $SQL_SERVER_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user $SQL_ADMIN_USER \
  --admin-password "$SQL_ADMIN_PASSWORD" \
  --output none

echo -e "${GREEN}? SQL Server created: $SQL_SERVER_NAME${NC}"

az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --name $SQL_DB_NAME \
  --service-objective S2 \
  --backup-storage-redundancy Local \
  --output none

echo -e "${GREEN}? SQL Database created: $SQL_DB_NAME (S2 tier)${NC}"

# Configure firewall
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0 \
  --output none

echo -e "${GREEN}? Firewall configured to allow Azure services${NC}"
echo ""

# Step 6: Create Storage Account
echo -e "${YELLOW}Step 6/10: Creating Azure Storage Account...${NC}"
az storage account create \
  --name $STORAGE_ACCOUNT_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku Standard_LRS \
  --kind StorageV2 \
  --output none

echo -e "${GREEN}? Storage Account created: $STORAGE_ACCOUNT_NAME${NC}"

# Get storage account key
STORAGE_KEY=$(az storage account keys list \
  --resource-group $RESOURCE_GROUP \
  --account-name $STORAGE_ACCOUNT_NAME \
  --query "[0].value" \
  --output tsv)

# Create file share
az storage share create \
  --name uploads \
  --account-name $STORAGE_ACCOUNT_NAME \
  --account-key "$STORAGE_KEY" \
  --quota 10 \
  --output none

echo -e "${GREEN}? File share created: uploads (10 GB)${NC}"
echo ""

# Step 7: Configure storage in Container Apps Environment
echo -e "${YELLOW}Step 7/10: Configuring storage mount...${NC}"
az containerapp env storage set \
  --name $ENVIRONMENT_NAME \
  --resource-group $RESOURCE_GROUP \
  --storage-name uploads \
  --azure-file-account-name $STORAGE_ACCOUNT_NAME \
  --azure-file-account-key "$STORAGE_KEY" \
  --azure-file-share-name uploads \
  --access-mode ReadWrite \
  --output none

echo -e "${GREEN}? Storage mount configured${NC}"
echo ""

# Step 8: Build and push Docker image
echo -e "${YELLOW}Step 8/10: Building and pushing Docker image...${NC}"
echo -e "${YELLOW}This may take 3-5 minutes...${NC}"

# Navigate to project root (parent of azure folder)
cd "$(dirname "$0")/.."

az acr build \
  --registry $ACR_NAME \
  --image contosouniversity:latest \
  --image contosouniversity:v1.0 \
  --file Dockerfile \
  . \
  --output table

echo -e "${GREEN}? Docker image built and pushed${NC}"
echo ""

# Step 9: Deploy Container App
echo -e "${YELLOW}Step 9/10: Deploying Container App...${NC}"

# Build connection string
SQL_CONNECTION_STRING="Server=tcp:${SQL_SERVER_NAME}.database.windows.net,1433;Database=${SQL_DB_NAME};User ID=${SQL_ADMIN_USER};Password=${SQL_ADMIN_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# Enable managed identity and get ACR credentials
az containerapp create \
  --name $CONTAINER_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --environment $ENVIRONMENT_NAME \
  --image ${ACR_NAME}.azurecr.io/contosouniversity:latest \
  --target-port 8080 \
  --ingress 'external' \
  --registry-server ${ACR_NAME}.azurecr.io \
  --registry-identity system \
  --cpu 1.0 \
  --memory 2.0Gi \
  --min-replicas 2 \
  --max-replicas 30 \
  --secrets \
    sql-connection-string="$SQL_CONNECTION_STRING" \
  --env-vars \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection=secretref:sql-connection-string \
    NotificationQueuePath='.\\Private$\\ContosoUniversityNotifications' \
  --output none

echo -e "${GREEN}? Container App deployed${NC}"
echo ""

# Step 10: Configure scaling rules
echo -e "${YELLOW}Step 10/10: Configuring auto-scaling...${NC}"

az containerapp update \
  --name $CONTAINER_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --scale-rule-name http-scaling \
  --scale-rule-type http \
  --scale-rule-http-concurrency 100 \
  --output none

echo -e "${GREEN}? Auto-scaling configured${NC}"
echo ""

# Enable managed identity for ACR pull
echo -e "${YELLOW}Configuring managed identity...${NC}"
PRINCIPAL_ID=$(az containerapp identity show \
  --name $CONTAINER_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query principalId \
  --output tsv)

# Grant ACR pull permission
az role assignment create \
  --assignee $PRINCIPAL_ID \
  --role AcrPull \
  --scope $(az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query id --output tsv) \
  --output none

echo -e "${GREEN}? Managed identity configured for ACR pull${NC}"
echo ""

# Get application URL
APP_URL=$(az containerapp show \
  --name $CONTAINER_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query properties.configuration.ingress.fqdn \
  --output tsv)

# Wait for app to be ready
echo -e "${YELLOW}Waiting for application to be ready...${NC}"
for i in {1..30}; do
    if curl -f -s "https://${APP_URL}/health" > /dev/null 2>&1; then
        echo -e "${GREEN}? Application is healthy${NC}"
        break
    fi
    if [ $i -eq 30 ]; then
        echo -e "${RED}Warning: Health check timeout. Check logs.${NC}"
    fi
    echo -n "."
    sleep 5
done
echo ""

# ============================================
# Deployment Summary
# ============================================
echo ""
echo -e "${GREEN}============================================${NC}"
echo -e "${GREEN}? Deployment Complete!${NC}"
echo -e "${GREEN}============================================${NC}"
echo ""
echo -e "${BLUE}Application URL:${NC}"
echo -e "  https://${APP_URL}"
echo ""
echo -e "${BLUE}Health Check:${NC}"
echo -e "  https://${APP_URL}/health"
echo ""
echo -e "${BLUE}Azure Portal:${NC}"
echo -e "  https://portal.azure.com/#@/resource/subscriptions/$(az account show --query id -o tsv)/resourceGroups/${RESOURCE_GROUP}/overview"
echo ""
echo -e "${BLUE}Resources Created:${NC}"
echo -e "  • Resource Group:    ${RESOURCE_GROUP}"
echo -e "  • Container Registry: ${ACR_NAME}.azurecr.io"
echo -e "  • Container App:     ${CONTAINER_APP_NAME}"
echo -e "  • SQL Server:        ${SQL_SERVER_NAME}.database.windows.net"
echo -e "  • SQL Database:      ${SQL_DB_NAME} (S2 tier)"
echo -e "  • Storage Account:   ${STORAGE_ACCOUNT_NAME}"
echo -e "  • Log Analytics:     ${LOG_WORKSPACE_NAME}"
echo ""
echo -e "${BLUE}Credentials (SECURE THESE):${NC}"
echo -e "  SQL Admin User:      ${SQL_ADMIN_USER}"
echo -e "  SQL Admin Password:  (saved in .deployment-credentials.txt)"
echo ""
echo -e "${BLUE}Scaling Configuration:${NC}"
echo -e "  Min Replicas:        2 (High Availability)"
echo -e "  Max Replicas:        30 (Auto-scaling)"
echo -e "  Scale Trigger:       100 concurrent HTTP requests"
echo ""
echo -e "${BLUE}Estimated Monthly Cost:${NC}"
echo -e "  Container Apps:      ~$100-200 (variable)"
echo -e "  SQL Database (S2):   ~$150"
echo -e "  Storage:             ~$20"
echo -e "  Container Registry:  ~$20"
echo -e "  Log Analytics:       ~$30"
echo -e "  ${GREEN}Total: ~$320-420/month${NC}"
echo ""
echo -e "${BLUE}Useful Commands:${NC}"
echo -e "  View logs:"
echo -e "    az containerapp logs tail --name ${CONTAINER_APP_NAME} --resource-group ${RESOURCE_GROUP} --follow"
echo ""
echo -e "  Scale manually:"
echo -e "    az containerapp update --name ${CONTAINER_APP_NAME} --resource-group ${RESOURCE_GROUP} --min-replicas 3 --max-replicas 20"
echo ""
echo -e "  View metrics:"
echo -e "    az containerapp show --name ${CONTAINER_APP_NAME} --resource-group ${RESOURCE_GROUP} --query properties.template.scale"
echo ""
echo -e "  Update image:"
echo -e "    az containerapp update --name ${CONTAINER_APP_NAME} --resource-group ${RESOURCE_GROUP} --image ${ACR_NAME}.azurecr.io/contosouniversity:v1.1"
echo ""
echo -e "${GREEN}?? Deployment successful!${NC}"
echo -e "${GREEN}Visit your application at: https://${APP_URL}${NC}"
echo ""
