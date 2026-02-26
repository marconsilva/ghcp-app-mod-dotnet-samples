# ?? ContosoUniversity - Azure Container Apps Architecture

## Overview

This document describes the **Azure Container Apps** deployment architecture for ContosoUniversity, optimized for **serverless container hosting** with automatic scaling and minimal operational overhead.

---

## ??? High-Level Architecture

```
                                    Internet
                                       ?
                                       ? HTTPS
                        ???????????????????????????????
                        ?   Azure Front Door          ?
                        ?   (Global Load Balancer)    ?
                        ?   - SSL/TLS Termination     ?
                        ?   - DDoS Protection         ?
                        ?   - WAF (Web App Firewall)  ?
                        ???????????????????????????????
                                       ?
                        ???????????????????????????????
                        ?  Container Apps Environment ?
                        ?  (Managed Kubernetes)       ?
                        ???????????????????????????????
                                       ?
                ???????????????????????????????????????????????
                ?                                              ?
                ?        Auto-Scaled Container Instances       ?
                ?              (0-30 instances)                ?
                ?                                              ?
        ??????????????????     ??????????????     ?????????????????????
        ?   Instance 1   ? ... ? Instance N ? ... ?   Instance 30     ?
        ? ?????????????? ?     ?            ?     ? ??????????????    ?
        ? ? .NET 9.0   ? ?     ?            ?     ? ? .NET 9.0   ?    ?
        ? ? Web App    ? ?     ?            ?     ? ? Web App    ?    ?
        ? ?????????????? ?     ?            ?     ? ??????????????    ?
        ?   Port: 8080   ?     ?            ?     ?   Port: 8080      ?
        ??????????????????     ??????????????     ?????????????????????
                ?                                              ?
                ????????????????????????????????????????????????
                                   ?
                    ???????????????????????????????
                    ?  Virtual Network (Optional) ?
                    ?  - Private endpoints        ?
                    ?  - Network isolation        ?
                    ???????????????????????????????
                                   ?
                ???????????????????????????????????????
                ?                                     ?
        ??????????????????                   ??????????????????
        ?  Azure SQL DB  ?                   ?  Azure Files   ?
        ?  (Managed)     ?                   ?  (File Share)  ?
        ?                ?                   ?                ?
        ?  • Auto-backup ?                   ?  • Uploads     ?
        ?  • HA built-in ?                   ?  • Materials   ?
        ?  • Geo-replica ?                   ?  • SMB/NFS     ?
        ??????????????????                   ??????????????????


??????????????????????????????????????????????????????????????????????
?                   Monitoring & Management                           ?
??????????????????????????????????????????????????????????????????????
?                                                                     ?
?  ????????????????  ????????????????  ?????????????????           ?
?  ? Application  ?  ?     Log      ?  ?    Azure      ?           ?
?  ?  Insights    ?  ?  Analytics   ?  ?   Monitor     ?           ?
?  ?              ?  ?              ?  ?               ?           ?
?  ?  • Traces    ?  ?  • Queries   ?  ?  • Metrics    ?           ?
?  ?  • Metrics   ?  ?  • Alerts    ?  ?  • Dashboards ?           ?
?  ?  • Live      ?  ?  • Export    ?  ?  • Alerts     ?           ?
?  ????????????????  ????????????????  ?????????????????           ?
?                                                                     ?
???????????????????????????????????????????????????????????????????????
```

---

## ?? Why Azure Container Apps?

### Advantages Over AKS

| Feature | AKS | Container Apps | Winner |
|:--------|:---:|:--------------:|:------:|
| **Management Overhead** | High | None | ?? Container Apps |
| **Setup Complexity** | Complex | Simple | ?? Container Apps |
| **Scaling** | Manual setup | Automatic | ?? Container Apps |
| **Cost at Low Traffic** | Fixed | Scale to zero | ?? Container Apps |
| **HTTPS Setup** | Manual | Automatic | ?? Container Apps |
| **Load Balancing** | Configure | Built-in | ?? Container Apps |
| **Control & Flexibility** | Full | Limited | ?? AKS |
| **Kubernetes Features** | All | Subset | ?? AKS |

### Best For ContosoUniversity ?

Container Apps is **ideal** for ContosoUniversity because:

1. ? **Simple Web Application** - Not complex microservices
2. ? **Variable Traffic** - Student enrollment periods have spikes
3. ? **Cost Sensitive** - Educational institution budget
4. ? **Small Team** - Limited DevOps resources
5. ? **Fast Time to Market** - Quick deployment needed

### Cost Comparison

**Azure Container Apps:**
```
Base Cost:           $0 (pay per use)
Typical Monthly:     $50-150 (variable traffic)
Low Traffic:         $20-40 (scales to zero)
High Traffic:        $200-300 (30 instances)

?? Savings: 60-80% vs AKS for variable workloads
```

**AKS:**
```
Base Cost:           $400-600/month (nodes always running)
Typical Monthly:     $600-800
Low Traffic:         $600 (still paying for nodes)
High Traffic:        $1,200+ (more nodes)
```

---

## ??? Container Apps Architecture Details

### Container Apps Environment

```
???????????????????????????????????????????????????????????????
?        Container Apps Environment (contoso-env)              ?
???????????????????????????????????????????????????????????????
?                                                              ?
?  ??????????????????????????????????????????????????????    ?
?  ?          Container App: contoso-web                ?    ?
?  ?                                                     ?    ?
?  ?  Revision 1:  contosouniversity:v1.0   (Active)    ?    ?
?  ?  Revision 2:  contosouniversity:v1.1   (Traffic:0%)?    ?
?  ?                                                     ?    ?
?  ?  Scaling Rules:                                    ?    ?
?  ?    • HTTP: 100 concurrent requests per instance   ?    ?
?  ?    • Min Replicas: 1                               ?    ?
?  ?    • Max Replicas: 30                              ?    ?
?  ?                                                     ?    ?
?  ?  Active Instances: 3 (current load)               ?    ?
?  ??????????????????????????????????????????????????????    ?
?                                                              ?
?  ??????????????????????????????????????????????????????    ?
?  ?             Managed Components                      ?    ?
?  ?                                                     ?    ?
?  ?  ? Ingress Controller (automatic)                 ?    ?
?  ?  ? Load Balancer (built-in)                       ?    ?
?  ?  ? SSL Certificate (managed)                      ?    ?
?  ?  ? DNS (automatic)                                ?    ?
?  ?  ? Monitoring (integrated)                        ?    ?
?  ??????????????????????????????????????????????????????    ?
?                                                              ?
?  ??????????????????????????????????????????????????????    ?
?  ?          Log Analytics Workspace                    ?    ?
?  ?    • Container logs                                ?    ?
?  ?    • Application logs                              ?    ?
?  ?    • System logs                                   ?    ?
?  ??????????????????????????????????????????????????????    ?
?                                                              ?
????????????????????????????????????????????????????????????????
```

### Scaling Behavior

```
Traffic Pattern Over 24 Hours:

Instances
   30 ?                    ????
      ?                   ?    ?
   20 ?                 ?        ?
      ?                ?          ?
   10 ?      ??      ?              ?      ??
      ?    ?    ?  ?                  ?  ?  ??
    1 ????      ??                      ??    ???
    0 ????????????????????????????????????????????
      0   3   6   9   12  15  18  21  24 Hours

      ??? ???    ??????      ??????    ???
    Night Low  Morning    Afternoon   Evening
             Peak        Peak          Low

?? Cost Impact:
- Night (0-1 instances): $0-5/night
- Day (3-10 instances): $30-60/day
- Peak (10-30 instances): $100-150/day

Average Monthly: $80-120 (vs $600 fixed for AKS)
```

### Auto-Scaling Rules

```yaml
Scale Rules:
  HTTP Rule:
    - Concurrent Requests: 100 per instance
    - If requests > 100: Add instance
    - If requests < 50: Remove instance
    - Scale up time: 10-30 seconds
    - Scale down time: 2-5 minutes
  
  Schedule Rule (Optional):
    - Scale up at 8 AM (start of school day)
    - Scale down at 6 PM (end of day)
    - Weekend: minimum 1 instance
    
  CPU Rule (Optional):
    - CPU > 70%: Add instance
    - CPU < 30%: Remove instance
```

---

## ?? Request Flow

```
User Request Flow (Detailed):

1. User ? https://contosouniversity.azurecontainerapps.io
   ?
2. Azure Front Door (Optional)
   • SSL/TLS Termination
   • DDoS Protection
   • Web Application Firewall
   • Caching (static assets)
   ?
3. Container Apps Ingress (Managed)
   • Automatic load balancing
   • Health check validation
   • Traffic splitting (blue-green)
   • Request routing
   ?
4. Container Instance Selection
   • Round-robin or least connections
   • Only healthy instances
   • Session affinity (optional)
   ?
5. Container App (.NET 9.0)
   • Health check: /health/ready
   • Process request
   • Access database
   • Access file storage
   ?
6. Data Access Layer
   ??? Azure SQL Database
   ?   • Connection pooling
   ?   • Query optimization
   ?   • Automatic failover
   ?
   ??? Azure Files
       • Upload/download files
       • Teaching materials
       • Student documents
   ?
7. Response Generation
   • Render view
   • Apply caching headers
   • Compress response
   ?
8. Return to User
   • HTML/JSON response
   • Status code
   • Headers
```

---

## ?? Data & Storage Architecture

```
??????????????????????????????????????????????????????????????????
?                    Data Architecture                            ?
??????????????????????????????????????????????????????????????????

Container App Instances (0-30)
        ?
        ? Stateless - No local storage
        ?
        ??????????????????????????????????????????????
        ?                                             ?
        ?                                             ?
???????????????????????                   ????????????????????????
?  Azure SQL Database ?                   ?    Azure Files       ?
?  (PaaS - Managed)   ?                   ?  (Managed Storage)   ?
???????????????????????                   ????????????????????????
?                     ?                   ?                      ?
?  Features:          ?                   ?  Features:           ?
?  ? Auto-scaling    ?                   ?  ? SMB/NFS protocol ?
?  ? Auto-backup     ?                   ?  ? Shared access    ?
?  ? Auto-patching   ?                   ?  ? Snapshots        ?
?  ? Point-in-time   ?                   ?  ? Geo-redundant    ?
?     restore         ?                   ?  ? 99.9% SLA        ?
?  ? Geo-replication ?                   ?                      ?
?  ? 99.99% SLA      ?                   ?  Mount Point:        ?
?                     ?                   ?  /app/wwwroot/Uploads?
?  Tier: Basic/S2/P1  ?                   ?                      ?
?  Size: 10-250 GB    ?                   ?  Size: 10-100 GB    ?
???????????????????????                   ????????????????????????
         ?                                            ?
         ? Automatic backups                          ? Automatic backups
         ?                                            ?
???????????????????????                   ????????????????????????
?  Geo-Replica (opt)  ?                   ?  Storage Snapshots   ?
?  Read-only copy     ?                   ?  Point-in-time       ?
?  Other region       ?                   ?  recovery            ?
???????????????????????                   ????????????????????????
```

---

## ?? Container Apps vs AKS Decision Matrix

### ContosoUniversity Requirements Analysis

| Requirement | Container Apps | AKS | Recommendation |
|:------------|:--------------:|:---:|:--------------:|
| **Simple web app** | ? Perfect fit | ?? Overkill | ?? Container Apps |
| **Variable traffic** | ? Scale to zero | ? Minimum cost | ?? Container Apps |
| **Small team** | ? No K8s expertise | ? Requires expertise | ?? Container Apps |
| **Fast deployment** | ? Minutes | ?? Hours | ?? Container Apps |
| **Budget conscious** | ? Low cost | ?? Higher cost | ?? Container Apps |
| **Maintenance** | ? Fully managed | ?? Requires ops | ?? Container Apps |

**Decision: Azure Container Apps** ?

### When to Consider AKS Instead

Choose AKS if you need:
- Complex microservices (10+ services)
- Service mesh (Istio, Linkerd)
- Advanced networking (network policies)
- Sidecar containers
- Custom ingress controllers
- Full Kubernetes control
- Already have Kubernetes expertise

**For ContosoUniversity:** None of these apply ?

---

## ??? Detailed Component Architecture

### 1. Container Apps Environment

```
Container Apps Environment: contoso-env
??? Region: East US (primary)
??? Virtual Network: (Optional) contoso-vnet
??? Log Analytics: contoso-logs
??? Monitoring: Enabled
??? Networking: External ingress enabled

Resource Group: contoso-rg
??? Location: eastus
??? Tags:
?   ??? Environment: Production
?   ??? Application: ContosoUniversity
?   ??? ManagedBy: Container Apps
```

### 2. Container App Configuration

```yaml
Container App: contoso-web
??? Image: contosoacr.azurecr.io/contosouniversity:latest
??? Port: 8080 (HTTP)
??? Ingress:
?   ??? External: Enabled
?   ??? Transport: HTTP/HTTPS
?   ??? Custom Domain: contosouniversity.com
?   ??? Certificates: Managed (automatic SSL)
??? Scale:
?   ??? Min Replicas: 1 (or 0 for scale-to-zero)
?   ??? Max Replicas: 30
?   ??? HTTP Rule: 100 concurrent requests
?   ??? CPU Rule: 70% threshold (optional)
??? Resources:
?   ??? CPU: 1.0 vCPU
?   ??? Memory: 2.0 Gi
?   ??? Ephemeral Storage: 1.0 Gi
??? Identity:
?   ??? Managed Identity: Enabled (for ACR pull)
??? Secrets:
    ??? sql-connection-string
    ??? storage-connection-string
```

### 3. Azure SQL Database

```yaml
SQL Server: contoso-sqlserver
??? Location: East US
??? Version: SQL Server 2022
??? Authentication: SQL + Azure AD
??? Firewall:
?   ??? Allow Azure Services: Yes
?
Database: ContosoUniversity
??? Tier: Standard S2
?   ??? DTUs: 50
?   ??? Max Size: 250 GB
?   ??? Backups: Automatic (35 days)
?   ??? Cost: ~$150/month
??? Connection:
?   ??? Endpoint: contoso-sqlserver.database.windows.net
?   ??? Port: 1433
?   ??? Encryption: Required
??? Features:
?   ??? Automatic tuning: Enabled
?   ??? Threat detection: Enabled
?   ??? Geo-replication: Optional ($300/month)
?   ??? Long-term retention: Optional
```

### 4. Azure Storage Account

```yaml
Storage Account: contosostorage
??? Location: East US
??? Replication: LRS (or GRS for HA)
??? Performance: Standard
??? Cost: ~$20-30/month
?
File Share: uploads
??? Protocol: SMB 3.0
??? Size: 10-100 GB
??? Mount Point: /app/wwwroot/Uploads
??? Access: Private (VNet or shared key)
??? Features:
?   ??? Snapshots: Enabled (daily)
?   ??? Soft delete: 7 days
?   ??? Versioning: Optional
?   ??? Backup: Azure Backup integration
```

### 5. Azure Container Registry

```yaml
Container Registry: contosoacr
??? Location: East US
??? SKU: Basic ($5/month) or Standard ($20/month)
??? Images:
?   ??? contosouniversity:latest
?   ??? contosouniversity:v1.0
?   ??? contosouniversity:v1.1
?   ??? contosouniversity:v1.2
??? Features:
?   ??? Geo-replication: No (not needed)
?   ??? Webhook: To Container Apps (auto-deploy)
?   ??? Vulnerability scanning: Optional
?   ??? Content trust: Optional
```

---

## ?? Deployment Flow

```
Developer Workflow:

1. Code Change
   ?
   ? git commit & push
   ?
2. GitHub Actions Triggered
   ?? Run tests
   ?? Build Docker image
   ?? Push to Azure Container Registry
   ?? Tag: latest + git SHA
   ?
   ? Webhook notification
   ?
3. Container Apps Auto-Update (Optional)
   ?? Pull new image
   ?? Create new revision
   ?? Run health checks
   ?? Traffic split: 0% ? 100%
   ?
   ? Zero-downtime switch
   ?
4. New Revision Active
   ?? Old revision: 0% traffic
   ?? New revision: 100% traffic
   ?? Old revision: Deactivated after 24h
   ?
   ?
5. Monitor & Validate
   ?? Application Insights metrics
   ?? Log Analytics queries
   ?? Health check validation


Rollback Flow (if needed):

Error Detected
   ?
   ?
Switch Traffic to Previous Revision
   ?
   ?
Traffic: 100% ? old revision (instant)
   ?
   ?
Fix issue & redeploy
```

---

## ?? Cost Optimization Strategy

### Scaling Strategy

```yaml
Development Environment:
  Min Replicas: 0  # Scale to zero when not in use
  Max Replicas: 3
  Cost: ~$20-40/month (only when testing)

Staging Environment:
  Min Replicas: 1  # Always available
  Max Replicas: 5
  Cost: ~$80-120/month

Production Environment:
  Min Replicas: 2  # High availability
  Max Replicas: 30  # Handle peak loads
  Cost: ~$150-300/month (variable)
```

### Cost Breakdown (Production)

```
Azure Container Apps:
??? Base infrastructure: $0
??? vCPU time: $0.000024/vCPU-second
??? Memory: $0.000003/GiB-second
??? Requests: First 2M free, then $0.40/million

Typical ContosoUniversity Usage:
??? Avg instances: 3
??? vCPU per instance: 1.0
??? Memory per instance: 2.0 Gi
??? Hours running: ~16 hours/day (scale to zero at night)
?
??? Monthly Cost: ~$150-200

Plus:
??? Azure SQL (S2): $150/month
??? Storage: $20/month
??? Container Registry: $5/month
??? Log Analytics: $30/month
??? Total: ~$355-405/month

?? Savings vs AKS: ~$200-250/month (40% less)
```

### Cost Optimization Tips

1. **Scale to Zero** during off-hours
   ```bash
   az containerapp update \
     --name contoso-web \
     --min-replicas 0
   ```

2. **Right-size Resources**
   - Start with 0.5 vCPU, 1.0 Gi memory
   - Monitor and adjust based on metrics

3. **Use Reserved Capacity** (if consistent load)
   - Save 20-30% with 1-year commitment

4. **Optimize Database Tier**
   - Start with Basic tier ($5/month) for dev
   - Use S2 ($150/month) for production
   - Consider serverless SQL for variable loads

---

## ?? Deployment Steps (Simplified)

### Step 1: Prerequisites (5 minutes)

```bash
# Install Azure CLI
# Windows: Download from https://aka.ms/installazurecliwindows
# Mac: brew install azure-cli
# Linux: curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Login to Azure
az login

# Set subscription
az account set --subscription "Your Subscription Name"

# Verify
az account show
```

### Step 2: Create Resources (10 minutes)

```bash
# Run the deployment script
cd C:\code\gbb\app_mod_steps\ContosoUniversity\azure
.\deploy-container-apps.sh
```

**The script creates:**
- ? Resource Group
- ? Container Registry
- ? Container Apps Environment
- ? Azure SQL Database
- ? Storage Account with file share
- ? Log Analytics Workspace
- ? Container App with your application

### Step 3: Verify (2 minutes)

```bash
# Get application URL
az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query properties.configuration.ingress.fqdn \
  --output tsv

# Test health endpoint
curl https://<your-app-url>/health

# View logs
az containerapp logs tail \
  --name contoso-web \
  --resource-group contoso-rg \
  --follow
```

### Step 4: Configure Custom Domain (Optional, 15 minutes)

```bash
# Add custom domain
az containerapp hostname add \
  --name contoso-web \
  --resource-group contoso-rg \
  --hostname contosouniversity.com

# Bind managed certificate
az containerapp hostname bind \
  --name contoso-web \
  --resource-group contoso-rg \
  --hostname contosouniversity.com \
  --environment contoso-env \
  --validation-method CNAME
```

---

## ?? Configuration Management

### Environment Variables (Container Apps)

```bash
# Set environment variables
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --set-env-vars \
    ASPNETCORE_ENVIRONMENT=Production \
    NotificationQueuePath='.\\Private$\\ContosoUniversityNotifications'

# View current configuration
az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query properties.template.containers[0].env
```

### Secrets (Container Apps)

```bash
# Add secret
az containerapp secret set \
  --name contoso-web \
  --resource-group contoso-rg \
  --secrets \
    sql-connection-string="Server=tcp:..."

# Use secret in environment variable
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --set-env-vars \
    ConnectionStrings__DefaultConnection=secretref:sql-connection-string

# List secrets
az containerapp secret list \
  --name contoso-web \
  --resource-group contoso-rg
```

### Storage Mount

```bash
# Create storage mount
az containerapp env storage set \
  --name contoso-env \
  --resource-group contoso-rg \
  --storage-name uploads \
  --azure-file-account-name contosostorage \
  --azure-file-account-key "<key>" \
  --azure-file-share-name uploads \
  --access-mode ReadWrite

# Mount in container
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --volume-mount \
    uploads:/app/wwwroot/Uploads
```

---

## ?? Monitoring & Observability

### Application Insights Integration

```bash
# Enable Application Insights
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --enable-app-insights \
  --app-insights-key "<instrumentation-key>"
```

**Automatic Telemetry:**
- ? Request rates and response times
- ? Dependency tracking (SQL, external APIs)
- ? Exception tracking
- ? Custom events and metrics
- ? Live metrics stream
- ? Application map

### Log Analytics Queries

```kusto
// View application logs
ContainerAppConsoleLogs_CL
| where ContainerAppName_s == "contoso-web"
| where TimeGenerated > ago(1h)
| order by TimeGenerated desc
| project TimeGenerated, Log_s

// Monitor scaling events
ContainerAppSystemLogs_CL
| where ContainerAppName_s == "contoso-web"
| where Log_s contains "scaling"
| order by TimeGenerated desc

// Check for errors
ContainerAppConsoleLogs_CL
| where ContainerAppName_s == "contoso-web"
| where Log_s contains "error" or Log_s contains "exception"
| order by TimeGenerated desc
| take 50

// HTTP request metrics
requests
| where cloud_RoleName == "contoso-web"
| summarize 
    RequestCount=count(),
    AvgDuration=avg(duration),
    P95Duration=percentile(duration, 95)
    by bin(timestamp, 5m)
| render timechart
```

### Azure Monitor Dashboards

Create custom dashboard with:
- Request rate (requests/second)
- Response time (P50, P95, P99)
- Error rate (%)
- Instance count (current replicas)
- CPU usage (%)
- Memory usage (%)
- Database connections
- Failed requests

---

## ?? Security Configuration

### Managed Identity Setup

```bash
# Enable managed identity
az containerapp identity assign \
  --name contoso-web \
  --resource-group contoso-rg \
  --system-assigned

# Get identity principal ID
PRINCIPAL_ID=$(az containerapp identity show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query principalId \
  --output tsv)

# Grant ACR pull permission
az role assignment create \
  --assignee $PRINCIPAL_ID \
  --role AcrPull \
  --scope $(az acr show --name contosoacr --query id --output tsv)

# Grant SQL Database access
az sql server ad-admin set \
  --resource-group contoso-rg \
  --server-name contoso-sqlserver \
  --display-name contoso-web \
  --object-id $PRINCIPAL_ID
```

### Network Security

```bash
# Enable VNet integration
az containerapp vnet create \
  --name contoso-env \
  --resource-group contoso-rg \
  --location eastus

# Restrict ingress to specific IPs (optional)
az containerapp ingress access-restriction set \
  --name contoso-web \
  --resource-group contoso-rg \
  --action Allow \
  --ip-address-range 10.0.0.0/8,172.16.0.0/12

# Enable private endpoint for SQL
az sql server vnet-rule create \
  --server contoso-sqlserver \
  --name container-apps-rule \
  --resource-group contoso-rg \
  --subnet <subnet-id>
```

---

## ?? Continuous Deployment

### Automated Deployment (Recommended)

```bash
# Enable continuous deployment from ACR
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --enable-acr-pull

# Or use GitHub Actions (already configured)
# Push to main branch ? automatic deployment
git push origin main
```

### Manual Deployment

```bash
# Build new image locally
docker build -t contosoacr.azurecr.io/contosouniversity:v1.1 .

# Push to ACR
az acr login --name contosoacr
docker push contosoacr.azurecr.io/contosouniversity:v1.1

# Update container app
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --image contosoacr.azurecr.io/contosouniversity:v1.1
```

### Blue-Green Deployment

```bash
# Create new revision with 0% traffic
az containerapp revision copy \
  --name contoso-web \
  --resource-group contoso-rg \
  --image contosoacr.azurecr.io/contosouniversity:v1.1 \
  --revision-suffix v1-1

# Test new revision
curl https://contoso-web--v1-1.azurecontainerapps.io/health

# Gradually shift traffic
az containerapp ingress traffic set \
  --name contoso-web \
  --resource-group contoso-rg \
  --revision-weight v1-0=50 v1-1=50

# After validation, move 100% to new
az containerapp ingress traffic set \
  --name contoso-web \
  --resource-group contoso-rg \
  --revision-weight v1-1=100

# Deactivate old revision
az containerapp revision deactivate \
  --name contoso-web \
  --resource-group contoso-rg \
  --revision v1-0
```

---

## ?? Performance Tuning

### Optimize for Performance

```bash
# Increase resources for better performance
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --cpu 1.5 \
  --memory 3.0Gi

# Adjust scaling for faster response
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --scale-rule-name http-rule \
  --scale-rule-type http \
  --scale-rule-http-concurrency 50  # More aggressive scaling
```

### Connection Pooling

Already configured in Entity Framework Core:
```csharp
// In Program.cs - optimized connection pool
builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => 
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(30);
        })
);
```

---

## ?? Best Practices for Container Apps

### DO ?

1. **Use Managed Identity** for ACR and Azure SQL
2. **Enable Application Insights** for monitoring
3. **Set appropriate scale limits** (min: 1-2, max: 30)
4. **Use secrets** for sensitive configuration
5. **Mount Azure Files** for persistent uploads
6. **Enable health probes** (already implemented)
7. **Use revision mode** for blue-green deployments
8. **Tag images** with version numbers
9. **Set resource limits** appropriate for your load
10. **Enable logging** to Log Analytics

### DON'T ?

1. ? Store state in container (use Azure SQL/Files)
2. ? Hardcode secrets in image
3. ? Use `latest` tag in production
4. ? Set min replicas to 0 in production (use 1-2)
5. ? Ignore resource limits (can cause instability)
6. ? Skip health check endpoints
7. ? Use large image sizes (>500 MB)
8. ? Run as root user
9. ? Forget to enable logging
10. ? Deploy without testing health endpoints

---

## ?? Security Best Practices

### Container Security

```bash
# Scan image for vulnerabilities
az acr task create \
  --name security-scan \
  --registry contosoacr \
  --context /dev/null \
  --image contosouniversity:latest \
  --cmd "mcr.microsoft.com/azure-cli:latest" \
  --schedule "0 0 * * *"  # Daily scan
```

### Network Security

```bash
# Restrict ingress to specific sources
az containerapp ingress access-restriction set \
  --name contoso-web \
  --resource-group contoso-rg \
  --action Allow \
  --ip-address-range <your-corporate-ip-range>

# Enable VNet integration for private connectivity
az containerapp vnet set \
  --name contoso-web \
  --resource-group contoso-rg \
  --vnet-name contoso-vnet \
  --subnet containerapp-subnet
```

### Azure Key Vault Integration

```bash
# Create Key Vault
az keyvault create \
  --name contoso-kv \
  --resource-group contoso-rg \
  --location eastus

# Store secrets
az keyvault secret set \
  --vault-name contoso-kv \
  --name sql-connection-string \
  --value "Server=tcp:..."

# Grant Container App access
az keyvault set-policy \
  --name contoso-kv \
  --object-id $PRINCIPAL_ID \
  --secret-permissions get list

# Reference in Container App
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --set-env-vars \
    ConnectionStrings__DefaultConnection=secretref:sql-connection-string
```

---

## ?? Monitoring Dashboard Setup

### Create Azure Dashboard

```bash
# Export metrics to dashboard
az portal dashboard create \
  --name ContosoUniversityDashboard \
  --resource-group contoso-rg \
  --input-path dashboard.json
```

**Dashboard Widgets:**
1. Request rate (last 24 hours)
2. Response time (P50, P95, P99)
3. Error rate (%)
4. Active instances
5. CPU usage
6. Memory usage
7. Database DTU usage
8. Storage usage

### Set Up Alerts

```bash
# Alert on high error rate
az monitor metrics alert create \
  --name high-error-rate \
  --resource-group contoso-rg \
  --scopes $(az containerapp show --name contoso-web --resource-group contoso-rg --query id -o tsv) \
  --condition "avg requests/failed > 10" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --action email your-email@example.com

# Alert on high CPU
az monitor metrics alert create \
  --name high-cpu \
  --resource-group contoso-rg \
  --scopes $(az containerapp show --name contoso-web --resource-group contoso-rg --query id -o tsv) \
  --condition "avg UsageNanoCores > 700000000" \
  --window-size 5m \
  --evaluation-frequency 1m
```

---

## ?? Summary

### Why Container Apps for ContosoUniversity?

| Factor | Score | Justification |
|:-------|:-----:|:--------------|
| **Simplicity** | 10/10 | No cluster management |
| **Cost** | 9/10 | Pay only for usage |
| **Scaling** | 10/10 | Automatic, including scale-to-zero |
| **Time to Deploy** | 10/10 | Minutes vs hours |
| **Maintenance** | 10/10 | Fully managed |
| **Team Learning Curve** | 10/10 | Minimal K8s knowledge needed |
| **Overall** | **59/60** | **?? Excellent Choice** |

### Next Steps

1. ? Run local tests: `.\quick-start.ps1`
2. ? Deploy to Azure: `./azure/deploy-container-apps.sh`
3. ? Configure custom domain
4. ? Set up monitoring
5. ? Enable CI/CD
6. ? Production deployment

---

**Architecture Version**: 2.0 - Container Apps Focused  
**Last Updated**: February 26, 2026  
**Recommended For**: ContosoUniversity ?  
**Platform**: .NET 9.0 | Azure Container Apps | Azure SQL
