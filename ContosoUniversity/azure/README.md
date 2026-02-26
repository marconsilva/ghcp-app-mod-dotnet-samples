# ?? Azure Deployment Scripts

## ?? Recommended: Container Apps

**Quick Deploy to Azure Container Apps (15 minutes):**

```powershell
# Windows
.\deploy-container-apps.ps1

# Linux/Mac
chmod +x deploy-container-apps.sh
./deploy-container-apps.sh
```

**What happens:**
1. Creates Azure resources (Resource Group, Container Registry, SQL, Storage)
2. Builds and pushes Docker image
3. Deploys to Azure Container Apps
4. Configures auto-scaling (2-30 instances)
5. Sets up monitoring
6. Provides application URL

**Cost**: ~$320-420/month  
**Expertise**: Minimal  
**Management**: Zero (fully managed)

---

## ?? Files

### Container Apps (Recommended) ??

- `deploy-container-apps.ps1` - Windows deployment
- `deploy-container-apps.sh` - Linux/Mac deployment

**Use When:**
- ? Simple web application
- ? Variable traffic
- ? Small team
- ? Budget conscious

### AKS (Alternative)

- `deploy-aks.sh` - Full Kubernetes cluster deployment

**Use When:**
- Complex microservices (10+)
- Need service mesh
- Have Kubernetes expertise
- Need full K8s control

---

## ?? Cost Comparison

| Deployment | Monthly Cost | Best For |
|:-----------|:------------:|:---------|
| **Container Apps** ?? | **$320-420** | ContosoUniversity ? |
| AKS | $750+ | Complex microservices |

**Savings with Container Apps**: $380/month (52% less) ??

---

## ?? Quick Decision

**Choose Container Apps if you answer "Yes" to ?3:**
- [ ] Simple web application?
- [ ] Traffic varies (not constant)?
- [ ] Small team (<5 people)?
- [ ] Budget is important?
- [ ] Want minimal management?

**ContosoUniversity**: 5/5 = **Container Apps** ??

---

## ?? Documentation

- [Container Apps Architecture](../.github/container/CONTAINER-APPS-ARCHITECTURE.md)
- [Container Apps vs AKS Guide](../.github/container/CONTAINER-APPS-VS-AKS.md)
- [Operations Guide](../.github/container/CONTAINER-APPS-OPERATIONS.md)
- [Deployment Checklist](../.github/container/DEPLOYMENT-CHECKLIST.md)

---

## ? Prerequisites

### Required
- [Azure CLI](https://aka.ms/installazurecliwindows)
- Azure subscription
- Docker (if building locally)

### Optional
- PowerShell 7+ (Windows)
- Bash (Linux/Mac)

---

## ?? Deploy Now

```powershell
# 1. Login to Azure
az login

# 2. Deploy (Windows)
.\deploy-container-apps.ps1

# 3. Wait 15-20 minutes

# 4. Access your application
# URL provided at end
```

**That's it!** ??

---

**Recommended**: Azure Container Apps ??  
**Cost**: $320-420/month  
**Time**: 15 minutes  
**Complexity**: Low
