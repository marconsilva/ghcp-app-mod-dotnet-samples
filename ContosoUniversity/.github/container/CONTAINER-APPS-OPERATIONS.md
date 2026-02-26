# ?? Azure Container Apps Operations Guide

## Overview

This guide covers day-to-day operations for managing ContosoUniversity on Azure Container Apps.

---

## ?? Quick Start

### Deploy for the First Time

```bash
# Windows
cd C:\code\gbb\app_mod_steps\ContosoUniversity\azure
.\deploy-container-apps.ps1

# Linux/Mac
cd /path/to/ContosoUniversity/azure
chmod +x deploy-container-apps.sh
./deploy-container-apps.sh
```

**Time**: ~15-20 minutes  
**Cost**: ~$320-420/month

---

## ?? Daily Operations

### View Application Status

```bash
# Get overview
az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --output table

# Get current replicas
az containerapp revision list \
  --name contoso-web \
  --resource-group contoso-rg \
  --query "[?properties.active].{Name:name, Replicas:properties.replicas, Traffic:properties.trafficWeight, Created:properties.createdTime}" \
  --output table

# Get URL
az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query properties.configuration.ingress.fqdn \
  --output tsv
```

### View Logs

```bash
# Tail logs (follow)
az containerapp logs tail \
  --name contoso-web \
  --resource-group contoso-rg \
  --follow

# View recent logs
az containerapp logs show \
  --name contoso-web \
  --resource-group contoso-rg \
  --tail 100

# Filter for errors
az containerapp logs show \
  --name contoso-web \
  --resource-group contoso-rg \
  --tail 100 \
  --type system \
  | grep -i error
```

### Check Health

```bash
# Get app URL
APP_URL=$(az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query properties.configuration.ingress.fqdn \
  --output tsv)

# Test health endpoints
curl https://$APP_URL/health
curl https://$APP_URL/health/ready
curl https://$APP_URL/health/live

# Expected response:
# {"status":"Healthy","timestamp":"2026-02-26T...","version":"1.0.0"}
```

---

## ?? Update & Deployment

### Update Application (New Version)

```bash
# Method 1: Automatic (push to ACR, app auto-updates)
cd /path/to/ContosoUniversity
docker build -t contosoacr.azurecr.io/contosouniversity:v1.1 .
az acr login --name contosoacr
docker push contosoacr.azurecr.io/contosouniversity:v1.1

az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --image contosoacr.azurecr.io/contosouniversity:v1.1

# Method 2: Using ACR build (recommended)
az acr build \
  --registry contosoacr \
  --image contosouniversity:v1.1 \
  --file Dockerfile \
  .

az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --image contosoacr.azurecr.io/contosouniversity:v1.1
```

### Blue-Green Deployment (Zero Downtime)

```bash
# Create new revision with new image
az containerapp revision copy \
  --name contoso-web \
  --resource-group contoso-rg \
  --image contosoacr.azurecr.io/contosouniversity:v1.1 \
  --revision-suffix v1-1

# Test new revision (gets 0% traffic initially)
NEW_REVISION_URL=$(az containerapp revision show \
  --name contoso-web--v1-1 \
  --resource-group contoso-rg \
  --query properties.fqdn \
  --output tsv)

curl https://$NEW_REVISION_URL/health

# Shift 10% traffic to test
az containerapp ingress traffic set \
  --name contoso-web \
  --resource-group contoso-rg \
  --revision-weight \
    contoso-web--v1-0=90 \
    contoso-web--v1-1=10

# Monitor for 10-15 minutes, check logs and metrics

# If good, shift 100% traffic
az containerapp ingress traffic set \
  --name contoso-web \
  --resource-group contoso-rg \
  --revision-weight contoso-web--v1-1=100

# Deactivate old revision
az containerapp revision deactivate \
  --resource-group contoso-rg \
  --revision contoso-web--v1-0
```

### Rollback to Previous Version

```bash
# List revisions
az containerapp revision list \
  --name contoso-web \
  --resource-group contoso-rg \
  --output table

# Activate previous revision
az containerapp revision activate \
  --resource-group contoso-rg \
  --revision contoso-web--v1-0

# Shift traffic immediately
az containerapp ingress traffic set \
  --name contoso-web \
  --resource-group contoso-rg \
  --revision-weight contoso-web--v1-0=100
```

---

## ?? Configuration Management

### Update Environment Variables

```bash
# Add or update environment variable
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --set-env-vars \
    ASPNETCORE_ENVIRONMENT=Production \
    NEW_SETTING=value

# View current environment variables
az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query properties.template.containers[0].env \
  --output table

# Remove environment variable
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --remove-env-vars NEW_SETTING
```

### Update Secrets

```bash
# Add/update secret
az containerapp secret set \
  --name contoso-web \
  --resource-group contoso-rg \
  --secrets \
    new-secret="secret-value"

# List secrets (values are hidden)
az containerapp secret list \
  --name contoso-web \
  --resource-group contoso-rg \
  --output table

# Remove secret
az containerapp secret remove \
  --name contoso-web \
  --resource-group contoso-rg \
  --secret-names new-secret
```

### Update Database Connection String

```bash
# Update connection string secret
NEW_CONNECTION_STRING="Server=tcp:new-server.database.windows.net,1433;..."

az containerapp secret set \
  --name contoso-web \
  --resource-group contoso-rg \
  --secrets sql-connection-string="$NEW_CONNECTION_STRING"

# Restart to pick up new value
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --image contosoacr.azurecr.io/contosouniversity:latest
```

---

## ?? Scaling Operations

### Manual Scaling

```bash
# Scale up immediately (busy period)
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --min-replicas 5 \
  --max-replicas 30

# Scale down (low traffic period)
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --min-replicas 1 \
  --max-replicas 10

# View current scale
az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query properties.template.scale \
  --output json
```

### Scale to Zero (Development)

```bash
# Enable scale to zero (saves cost in dev)
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --min-replicas 0 \
  --max-replicas 5

# Note: First request after scale-to-zero may take 10-20 seconds
```

### Update Scaling Rules

```bash
# More aggressive scaling (50 concurrent requests)
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --scale-rule-name http-scaling \
  --scale-rule-type http \
  --scale-rule-http-concurrency 50

# Add CPU-based scaling
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --scale-rule-name cpu-scaling \
  --scale-rule-type cpu \
  --scale-rule-cpu-utilization 70

# Remove scaling rule
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --remove-scale-rule cpu-scaling
```

---

## ?? Resource Management

### Update CPU and Memory

```bash
# Increase resources
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --cpu 1.5 \
  --memory 3.0Gi

# Decrease resources (save cost)
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --cpu 0.5 \
  --memory 1.0Gi

# View current resource allocation
az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query properties.template.containers[0].resources
```

### Manage Revisions

```bash
# List all revisions
az containerapp revision list \
  --name contoso-web \
  --resource-group contoso-rg \
  --output table

# Get specific revision details
az containerapp revision show \
  --name contoso-web \
  --revision contoso-web--v1-0 \
  --resource-group contoso-rg

# Activate old revision
az containerapp revision activate \
  --resource-group contoso-rg \
  --revision contoso-web--v1-0

# Deactivate revision
az containerapp revision deactivate \
  --resource-group contoso-rg \
  --revision contoso-web--v1-0

# Set revision mode
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --revision-mode single  # or 'multiple' for blue-green
```

---

## ?? Monitoring & Diagnostics

### View Metrics

```bash
# Get container app metrics
az monitor metrics list \
  --resource $(az containerapp show --name contoso-web --resource-group contoso-rg --query id -o tsv) \
  --metric "Requests" \
  --start-time $(date -u -d '1 hour ago' '+%Y-%m-%dT%H:%M:%SZ') \
  --interval PT1M \
  --output table

# View replica count over time
az monitor metrics list \
  --resource $(az containerapp show --name contoso-web --resource-group contoso-rg --query id -o tsv) \
  --metric "Replicas" \
  --interval PT5M \
  --output table
```

### Log Analytics Queries

```bash
# Open Log Analytics in browser
az monitor log-analytics workspace show \
  --resource-group contoso-rg \
  --workspace-name contoso-logs \
  --query id \
  --output tsv \
  | xargs -I {} open "https://portal.azure.com/#@/resource{}/logs"
```

**Useful Queries:**

```kusto
// Application logs (last hour)
ContainerAppConsoleLogs_CL
| where ContainerAppName_s == "contoso-web"
| where TimeGenerated > ago(1h)
| project TimeGenerated, Log_s
| order by TimeGenerated desc

// Error logs
ContainerAppConsoleLogs_CL
| where ContainerAppName_s == "contoso-web"
| where Log_s contains "error" or Log_s contains "exception"
| order by TimeGenerated desc
| take 100

// Scaling events
ContainerAppSystemLogs_CL
| where ContainerAppName_s == "contoso-web"
| where Log_s contains "scaling" or Log_s contains "replica"
| project TimeGenerated, Log_s
| order by TimeGenerated desc

// Request count by status code
requests
| where cloud_RoleName == "contoso-web"
| summarize count() by resultCode
| render piechart

// Performance over time
requests
| where cloud_RoleName == "contoso-web"
| summarize 
    Requests=count(),
    AvgDuration=avg(duration),
    P95=percentile(duration, 95)
    by bin(timestamp, 5m)
| render timechart
```

---

## ??? Security Operations

### Rotate Secrets

```bash
# Generate new SQL password
NEW_PASSWORD=$(openssl rand -base64 24 | tr -d "=+/" | cut -c1-20)Aa1!

# Update SQL Server password
az sql server update \
  --name contoso-sqlserver \
  --resource-group contoso-rg \
  --admin-password "$NEW_PASSWORD"

# Update Container App secret
NEW_CONNECTION_STRING="Server=tcp:contoso-sqlserver.database.windows.net,1433;Database=ContosoUniversity;User ID=sqladmin;Password=${NEW_PASSWORD};..."

az containerapp secret set \
  --name contoso-web \
  --resource-group contoso-rg \
  --secrets sql-connection-string="$NEW_CONNECTION_STRING"

# Restart app to pick up new secret
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --image contosoacr.azurecr.io/contosouniversity:latest
```

### Enable Azure AD Authentication

```bash
# Enable authentication
az containerapp auth update \
  --name contoso-web \
  --resource-group contoso-rg \
  --enabled true \
  --unauthenticated-client-action RedirectToLoginPage \
  --redirect-provider azureactivedirectory

# Configure Azure AD
az containerapp auth microsoft update \
  --name contoso-web \
  --resource-group contoso-rg \
  --client-id <your-app-registration-id> \
  --client-secret <your-client-secret> \
  --issuer https://login.microsoftonline.com/<tenant-id>/v2.0
```

---

## ?? Backup & Disaster Recovery

### Database Backups

```bash
# View backup retention policy
az sql db show \
  --resource-group contoso-rg \
  --server contoso-sqlserver \
  --name ContosoUniversity \
  --query backupStorageRedundancy

# Create long-term retention policy
az sql db ltr-policy set \
  --resource-group contoso-rg \
  --server contoso-sqlserver \
  --database ContosoUniversity \
  --weekly-retention P4W \
  --monthly-retention P12M \
  --yearly-retention P5Y \
  --week-of-year 1

# List backups
az sql db ltr-backup list \
  --location eastus \
  --server contoso-sqlserver \
  --database ContosoUniversity
```

### Restore Database

```bash
# Point-in-time restore
az sql db restore \
  --resource-group contoso-rg \
  --server contoso-sqlserver \
  --name ContosoUniversity \
  --dest-name ContosoUniversity-Restored \
  --time "2026-02-26T12:00:00Z"

# Restore from long-term backup
az sql db ltr-backup restore \
  --dest-database ContosoUniversity-Restored \
  --dest-resource-group contoso-rg \
  --dest-server contoso-sqlserver \
  --backup-id "/subscriptions/.../backupId"
```

### File Storage Backups

```bash
# Create snapshot
az storage share snapshot \
  --name uploads \
  --account-name contosostorage

# List snapshots
az storage share list \
  --account-name contosostorage \
  --include-snapshot \
  --output table

# Restore from snapshot
az storage file copy start \
  --source-share uploads \
  --source-path "path/to/file" \
  --snapshot "2026-02-26T12:00:00.0000000Z" \
  --destination-share uploads \
  --destination-path "path/to/file"
```

---

## ?? Performance Optimization

### Analyze Performance

```bash
# View resource usage
az monitor metrics list \
  --resource $(az containerapp show --name contoso-web --resource-group contoso-rg --query id -o tsv) \
  --metric "UsageNanoCores,WorkingSetBytes" \
  --start-time $(date -u -d '6 hours ago' '+%Y-%m-%dT%H:%M:%SZ') \
  --interval PT5M \
  --output table

# Check if app is under-provisioned
# If CPU consistently > 80% or Memory > 85%, increase resources
```

### Optimize Resources

```bash
# If app uses < 50% CPU and < 60% memory, reduce:
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --cpu 0.75 \
  --memory 1.5Gi

# If app is slow or throttled, increase:
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --cpu 1.5 \
  --memory 3.0Gi
```

### Optimize Database

```bash
# View database performance
az sql db show \
  --resource-group contoso-rg \
  --server contoso-sqlserver \
  --name ContosoUniversity \
  --query "currentServiceObjectiveName"

# Scale up database (if needed)
az sql db update \
  --resource-group contoso-rg \
  --server contoso-sqlserver \
  --name ContosoUniversity \
  --service-objective S3  # or P1, P2 for Premium

# Scale down (cost savings)
az sql db update \
  --resource-group contoso-rg \
  --server contoso-sqlserver \
  --name ContosoUniversity \
  --service-objective S0  # Basic tier
```

---

## ?? Troubleshooting

### Application Won't Start

```bash
# Check revision status
az containerapp revision list \
  --name contoso-web \
  --resource-group contoso-rg

# View system logs
az containerapp logs show \
  --name contoso-web \
  --resource-group contoso-rg \
  --type system \
  --tail 50

# Common issues:
# 1. Image pull failed - check ACR access
# 2. Health check failed - check /health endpoint
# 3. Configuration error - check env vars and secrets
```

### Database Connection Issues

```bash
# Test firewall rules
az sql server firewall-rule list \
  --resource-group contoso-rg \
  --server contoso-sqlserver \
  --output table

# Add your IP temporarily for testing
MY_IP=$(curl -s https://api.ipify.org)
az sql server firewall-rule create \
  --resource-group contoso-rg \
  --server contoso-sqlserver \
  --name MyIP \
  --start-ip-address $MY_IP \
  --end-ip-address $MY_IP

# Test connection from local machine
sqlcmd -S contoso-sqlserver.database.windows.net -U sqladmin -P <password> -Q "SELECT 1"
```

### High CPU/Memory Usage

```bash
# Check current usage
az monitor metrics list \
  --resource $(az containerapp show --name contoso-web --resource-group contoso-rg --query id -o tsv) \
  --metric "UsageNanoCores,WorkingSetBytes" \
  --output table

# If consistently high:
# 1. Scale out (more replicas)
az containerapp update --name contoso-web --resource-group contoso-rg --min-replicas 5

# 2. Scale up (more resources per replica)
az containerapp update --name contoso-web --resource-group contoso-rg --cpu 1.5 --memory 3.0Gi

# 3. Check for memory leaks or inefficient queries
```

### Slow Performance

```bash
# Check Application Insights
az monitor app-insights component show \
  --app contoso-web \
  --resource-group contoso-rg

# View slow requests in Log Analytics
# Query:
requests
| where cloud_RoleName == "contoso-web"
| where duration > 1000  # > 1 second
| order by duration desc
| take 20
```

---

## ?? Cost Management

### View Current Costs

```bash
# Get cost for last 30 days
az consumption usage list \
  --start-date $(date -u -d '30 days ago' '+%Y-%m-%d') \
  --end-date $(date -u '+%Y-%m-%d') \
  | jq '[.[] | select(.instanceName | contains("contoso"))] | group_by(.meterCategory) | map({category: .[0].meterCategory, cost: map(.pretaxCost) | add})'

# Set budget alert
az consumption budget create \
  --budget-name contoso-monthly-budget \
  --amount 500 \
  --resource-group contoso-rg \
  --time-grain Monthly \
  --start-date $(date -u '+%Y-%m-01') \
  --end-date $(date -u -d '+1 year' '+%Y-%m-%d')
```

### Cost Optimization

```bash
# Review and optimize:

# 1. Scale down during off-hours (example: nights and weekends)
# Option A: Manual
az containerapp update --name contoso-web --resource-group contoso-rg --min-replicas 0

# Option B: Azure Automation (scheduled)
# Create runbook to scale based on schedule

# 2. Reduce SQL tier if over-provisioned
az sql db update \
  --resource-group contoso-rg \
  --server contoso-sqlserver \
  --name ContosoUniversity \
  --service-objective S1  # From S2 to S1 saves ~$75/month

# 3. Use reserved capacity (1-year commit)
# Available in Azure Portal under SQL Database ? Reserved Capacity
# Savings: 20-30%

# 4. Enable storage lifecycle management
az storage account management-policy create \
  --account-name contosostorage \
  --resource-group contoso-rg \
  --policy @storage-lifecycle-policy.json
```

---

## ?? Security Hardening

### Enable VNet Integration

```bash
# Create VNet
az network vnet create \
  --name contoso-vnet \
  --resource-group contoso-rg \
  --address-prefix 10.0.0.0/16 \
  --subnet-name container-subnet \
  --subnet-prefix 10.0.0.0/23

# Enable VNet integration
az containerapp vnet set \
  --name contoso-web \
  --resource-group contoso-rg \
  --vnet-name contoso-vnet \
  --subnet container-subnet
```

### Restrict Access

```bash
# Restrict ingress to specific IPs
az containerapp ingress access-restriction set \
  --name contoso-web \
  --resource-group contoso-rg \
  --action Allow \
  --ip-address-range 203.0.113.0/24  # Your corporate IP range

# List current restrictions
az containerapp ingress access-restriction list \
  --name contoso-web \
  --resource-group contoso-rg
```

### Enable Managed Identity for SQL

```bash
# Get managed identity
PRINCIPAL_ID=$(az containerapp identity show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query principalId \
  --output tsv)

# Set as SQL Server AD admin
az sql server ad-admin create \
  --resource-group contoso-rg \
  --server-name contoso-sqlserver \
  --display-name contoso-web-identity \
  --object-id $PRINCIPAL_ID

# Update connection string to use managed identity
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --set-env-vars \
    ConnectionStrings__DefaultConnection="Server=tcp:contoso-sqlserver.database.windows.net,1433;Database=ContosoUniversity;Authentication=Active Directory Managed Identity;Encrypt=True;"
```

---

## ?? Maintenance Tasks

### Clean Up Old Revisions

```bash
# List inactive revisions
az containerapp revision list \
  --name contoso-web \
  --resource-group contoso-rg \
  --query "[?properties.active==\`false\`].name" \
  --output tsv

# Deactivate old revisions (keeps last 3)
az containerapp revision list \
  --name contoso-web \
  --resource-group contoso-rg \
  --query "[?properties.active==\`false\`].name" \
  --output tsv \
  | head -n -3 \
  | xargs -I {} az containerapp revision deactivate --revision {} --resource-group contoso-rg
```

### Clean Up Old Images

```bash
# List images in ACR
az acr repository list \
  --name contosoacr \
  --output table

# List tags
az acr repository show-tags \
  --name contosoacr \
  --repository contosouniversity \
  --output table

# Delete old tags (keep last 10)
az acr repository show-tags \
  --name contosoacr \
  --repository contosouniversity \
  --orderby time_desc \
  --output tsv \
  | tail -n +11 \
  | xargs -I {} az acr repository delete \
    --name contosoacr \
    --image contosouniversity:{} \
    --yes
```

### Update Base Image

```bash
# Rebuild with latest base image
cd /path/to/ContosoUniversity

docker build \
  --no-cache \
  --pull \
  -t contosoacr.azurecr.io/contosouniversity:latest \
  .

docker push contosoacr.azurecr.io/contosouniversity:latest

# Deploy updated image
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --image contosoacr.azurecr.io/contosouniversity:latest
```

---

## ?? Alerts & Notifications

### Create Alert Rules

```bash
# High error rate alert
az monitor metrics alert create \
  --name high-error-rate \
  --resource-group contoso-rg \
  --scopes $(az containerapp show --name contoso-web --resource-group contoso-rg --query id -o tsv) \
  --condition "avg requests/failed > 10" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --description "Alert when error rate exceeds 10 requests/5min" \
  --severity 2

# High response time alert
az monitor metrics alert create \
  --name slow-response \
  --resource-group contoso-rg \
  --scopes $(az containerapp show --name contoso-web --resource-group contoso-rg --query id -o tsv) \
  --condition "avg requests/duration > 2000" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --description "Alert when avg response time > 2 seconds" \
  --severity 3

# Application down alert
az monitor metrics alert create \
  --name app-down \
  --resource-group contoso-rg \
  --scopes $(az containerapp show --name contoso-web --resource-group contoso-rg --query id -o tsv) \
  --condition "max Replicas == 0" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --description "Alert when all replicas are down" \
  --severity 1
```

### Configure Action Groups

```bash
# Create action group for email notifications
az monitor action-group create \
  --name contoso-alerts \
  --resource-group contoso-rg \
  --short-name contoso \
  --email-receiver \
    name=DevOpsTeam \
    email-address=devops@example.com

# Add action group to alert
az monitor metrics alert update \
  --name high-error-rate \
  --resource-group contoso-rg \
  --add-action contoso-alerts
```

---

## ?? Common Scenarios

### Scenario 1: Expected Traffic Spike

```bash
# Before event (e.g., enrollment period)
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --min-replicas 10 \
  --max-replicas 30

# After event
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --min-replicas 2 \
  --max-replicas 10
```

### Scenario 2: Maintenance Window

```bash
# Stop accepting new traffic (set min=0, wait for graceful shutdown)
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --min-replicas 0

# Perform maintenance (database, etc.)
# ...

# Resume service
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --min-replicas 2
```

### Scenario 3: Emergency Rollback

```bash
# Immediately switch traffic to previous revision
az containerapp ingress traffic set \
  --name contoso-web \
  --resource-group contoso-rg \
  --revision-weight contoso-web--v1-0=100

# Or rollback to specific image
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --image contosoacr.azurecr.io/contosouniversity:v1.0
```

---

## ?? Reporting

### Generate Usage Report

```bash
# Get revision history
az containerapp revision list \
  --name contoso-web \
  --resource-group contoso-rg \
  --query "[].{Name:name, Created:properties.createdTime, Active:properties.active, Replicas:properties.replicas}" \
  --output table

# Get scaling history from Log Analytics
# Use query:
ContainerAppSystemLogs_CL
| where ContainerAppName_s == "contoso-web"
| where Log_s contains "scaling"
| project TimeGenerated, Log_s
| order by TimeGenerated desc

# Export to CSV
az monitor metrics list \
  --resource $(az containerapp show --name contoso-web --resource-group contoso-rg --query id -o tsv) \
  --metric "Replicas" \
  --start-time "2026-02-01" \
  --end-time "2026-02-28" \
  --interval PT1H \
  --output json \
  > replica-count-february.json
```

---

## ?? Continuous Deployment Setup

### Enable GitHub Actions Integration

```bash
# Get registry credentials
ACR_USERNAME=$(az acr credential show --name contosoacr --query username -o tsv)
ACR_PASSWORD=$(az acr credential show --name contosoacr --query passwords[0].value -o tsv)

echo "Add these as GitHub Secrets:"
echo "AZURE_CONTAINER_REGISTRY: ${ACR_NAME}.azurecr.io"
echo "AZURE_REGISTRY_USERNAME: $ACR_USERNAME"
echo "AZURE_REGISTRY_PASSWORD: $ACR_PASSWORD"
echo "AZURE_CONTAINER_APP_NAME: $CONTAINER_APP_NAME"
echo "AZURE_RESOURCE_GROUP: $RESOURCE_GROUP"
```

### Set Up Webhook (Auto-deploy on image push)

```bash
# Create webhook
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --revision-mode multiple

# Container App will automatically create new revision when ACR image is updated
```

---

## ?? Support

### Get Help

```bash
# Azure support
az support ticket create \
  --ticket-name "Container Apps Issue" \
  --title "ContosoUniversity Container App Issue" \
  --description "Description of issue" \
  --severity minimal \
  --problem-classification-id "..." \
  --contact-first-name "Your" \
  --contact-last-name "Name" \
  --contact-method email \
  --contact-email your-email@example.com

# Community support
# https://github.com/microsoft/azure-container-apps
```

---

## ??? Cleanup (Delete Everything)

```bash
# WARNING: This deletes ALL resources and data

# Delete resource group (removes everything)
az group delete \
  --name contoso-rg \
  --yes \
  --no-wait

# Or delete individual resources
az containerapp delete --name contoso-web --resource-group contoso-rg --yes
az sql server delete --name contoso-sqlserver --resource-group contoso-rg --yes
az storage account delete --name contosostorage --resource-group contoso-rg --yes
az acr delete --name contosoacr --resource-group contoso-rg --yes
```

---

## ?? Additional Resources

- [Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [Container Apps Samples](https://github.com/Azure-Samples/container-apps-samples)
- [Azure SQL Documentation](https://learn.microsoft.com/azure/azure-sql/)
- [Application Insights](https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview)

---

**Guide Version**: 1.0  
**Last Updated**: February 26, 2026  
**Platform**: Azure Container Apps | .NET 9.0
