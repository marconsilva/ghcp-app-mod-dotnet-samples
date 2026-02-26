# ?? ContosoUniversity - Containerized

> A cloud-native, containerized version of the ContosoUniversity application built with .NET 9.0

## ?? Overview

This repository contains a fully containerized version of the ContosoUniversity application, ready for deployment on Docker, Kubernetes, Azure Container Apps, and Azure Kubernetes Service (AKS).

### ? Features

- ? Multi-stage Dockerfile for optimized image size
- ? Docker Compose for local development
- ? Kubernetes manifests for production deployment
- ? Azure deployment scripts (Container Apps & AKS)
- ? Health check endpoints for container orchestration
- ? Horizontal pod autoscaling
- ? CI/CD pipelines (Azure Pipelines & GitHub Actions)
- ? Production-ready security configurations
- ? Persistent storage for uploads
- ? Comprehensive monitoring and logging

---

## ?? Quick Start

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop) 24.0+
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- (Optional) [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) for Azure deployments
- (Optional) [kubectl](https://kubernetes.io/docs/tasks/tools/) for Kubernetes

### Run Locally with Docker Compose

**Option 1: Quick Start Script (Recommended)**

```bash
# Linux/Mac
chmod +x quick-start.sh
./quick-start.sh

# Windows PowerShell
.\quick-start.ps1
```

**Option 2: Manual Start**

```bash
# Start the application
docker-compose up -d

# View logs
docker-compose logs -f

# Access the application
open http://localhost:8080

# Stop the application
docker-compose down
```

### Build Docker Image

```bash
# Build the image
docker build -t contosouniversity:latest .

# Run the container
docker run -d -p 8080:8080 contosouniversity:latest
```

---

## ?? Project Structure

```
ContosoUniversity/
??? Dockerfile                          # Multi-stage Docker build
??? .dockerignore                       # Docker ignore file
??? docker-compose.yml                  # Development compose
??? docker-compose.prod.yml             # Production compose
??? quick-start.sh                      # Quick start (Linux/Mac)
??? quick-start.ps1                     # Quick start (Windows)
??? azure-pipelines.yml                 # Azure Pipelines CI/CD
??? .github/
?   ??? workflows/
?       ??? docker-build.yml            # GitHub Actions workflow
??? kubernetes/
?   ??? deployment.yaml                 # K8s deployment manifest
?   ??? ingress.yaml                    # K8s ingress configuration
??? azure/
?   ??? deploy-container-apps.sh        # Azure Container Apps deploy
?   ??? deploy-aks.sh                   # AKS deployment script
??? .github/
    ??? container/
        ??? CONTAINERIZATION-GUIDE.md   # Detailed guide
```

---

## ??? Architecture

### Container Architecture

```
???????????????????????????????????????????????????????????????
?                     Load Balancer / Ingress                  ?
???????????????????????????????????????????????????????????????
                         ?
        ???????????????????????????????????
        ?                                 ?
??????????????????              ??????????????????
?  Web Pod 1     ?              ?  Web Pod 2     ?
?  - App (8080)  ?              ?  - App (8080)  ?
?  - Health API  ?              ?  - Health API  ?
??????????????????              ??????????????????
        ?                                 ?
        ???????????????????????????????????
                         ?
          ???????????????????????????????
          ?                             ?
    ?????????????               ???????????????
    ? SQL Server ?               ? File Storage?
    ? Container  ?               ?  (Volume)   ?
    ??????????????               ???????????????
```

### Image Layers

```
Base Image (aspnet:9.0)     ~210 MB
Application Files            ~40 MB
Runtime Dependencies         ~10 MB
??????????????????????????????????
Final Image                 ~260 MB
```

---

## ?? Kubernetes Deployment

### Quick Deploy

```bash
# Create namespace
kubectl create namespace contosouniversity

# Deploy application
kubectl apply -f kubernetes/

# Check status
kubectl get all -n contosouniversity

# Access application
kubectl port-forward service/contoso-web-service 8080:80 -n contosouniversity
```

### Features

- **Auto-scaling**: 3-10 replicas based on CPU/Memory
- **Health Checks**: Liveness, readiness, and startup probes
- **Persistent Storage**: For uploads and SQL data
- **Load Balancing**: Automatic with Kubernetes Service
- **Rolling Updates**: Zero-downtime deployments

---

## ?? Azure Deployment

### Azure Container Apps (Serverless)

Simplest deployment option with automatic scaling:

```bash
cd azure
chmod +x deploy-container-apps.sh
./deploy-container-apps.sh
```

**Features:**
- ? Serverless container hosting
- ? Automatic HTTPS
- ? Built-in load balancing
- ? Scale to zero support
- ? Integrated with Azure SQL

**Cost**: ~$50-100/month (based on usage)

### Azure Kubernetes Service (AKS)

Full Kubernetes cluster for production workloads:

```bash
cd azure
chmod +x deploy-aks.sh
./deploy-aks.sh
```

**Features:**
- ? Full Kubernetes control
- ? Node autoscaling
- ? Azure Monitor integration
- ? Private endpoints
- ? Network policies

**Cost**: ~$600/month (3-node cluster)

---

## ?? Configuration

### Environment Variables

| Variable | Description | Required |
|:---------|:------------|:--------:|
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | Yes |
| `ConnectionStrings__DefaultConnection` | Database connection string | Yes |
| `ASPNETCORE_URLS` | HTTP binding URLs | No |
| `NotificationQueuePath` | MSMQ queue path | No |

### Connection Strings

**Development (Docker Compose)**
```
Server=sqlserver;Database=ContosoUniversity;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;
```

**Production (Azure SQL)**
```
Server=tcp:server.database.windows.net,1433;Database=ContosoUniversity;User ID=user;Password=pass;Encrypt=True;
```

---

## ?? Health Checks

The application exposes several health check endpoints:

| Endpoint | Purpose | Used By |
|:---------|:--------|:--------|
| `/health` | Basic health check | Docker HEALTHCHECK |
| `/health/live` | Liveness probe | Kubernetes liveness |
| `/health/ready` | Readiness probe | Kubernetes readiness |
| `/health/startup` | Startup probe | Kubernetes startup |

### Test Health Checks

```bash
# Basic health
curl http://localhost:8080/health

# Readiness (includes DB check)
curl http://localhost:8080/health/ready

# Liveness
curl http://localhost:8080/health/live
```

---

## ?? Monitoring & Logging

### View Logs

**Docker**
```bash
# View logs
docker logs contoso-web

# Follow logs
docker logs -f contoso-web

# Docker Compose
docker-compose logs -f web
```

**Kubernetes**
```bash
# View pod logs
kubectl logs -f deployment/contoso-web -n contosouniversity

# View specific pod
kubectl logs <pod-name> -n contosouniversity

# Stream logs from all pods
kubectl logs -f -l app=contoso-web -n contosouniversity
```

### Azure Monitor

When deployed to Azure, the application automatically integrates with:
- **Application Insights** - Application performance monitoring
- **Log Analytics** - Centralized logging
- **Azure Monitor** - Metrics and alerts

---

## ?? CI/CD Pipelines

### GitHub Actions

Automatically builds and deploys on push to `main` or `develop`:

- ? Runs tests
- ? Builds Docker image
- ? Pushes to container registry
- ? Deploys to Kubernetes
- ? Runs smoke tests

Configured in `.github/workflows/docker-build.yml`

### Azure Pipelines

Enterprise CI/CD with Azure DevOps:

- ? Multi-stage pipeline
- ? Code coverage reports
- ? Security scanning
- ? Environment approvals
- ? Rollback support

Configured in `azure-pipelines.yml`

---

## ?? Security

### Security Features

- ? Non-root user in container
- ? Read-only root filesystem (where possible)
- ? Minimal base image (aspnet runtime only)
- ? No secrets in image layers
- ? TLS/SSL for external traffic
- ? Network policies in Kubernetes
- ? Pod security standards

### Security Best Practices

1. **Secrets Management**
   - Use Kubernetes Secrets
   - Use Azure Key Vault for production
   - Never commit secrets to git

2. **Network Security**
   - Enable network policies
   - Use private endpoints for Azure services
   - Restrict ingress to necessary ports

3. **Image Security**
   - Regular security scans
   - Update base images regularly
   - Use specific image tags, not `latest`

---

## ??? Troubleshooting

### Common Issues

#### Container won't start
```bash
# Check logs
docker logs contoso-web

# Inspect container
docker inspect contoso-web

# Common causes:
# - Missing environment variables
# - Database connection issues
# - Port conflicts
```

#### Database connection fails
```bash
# Test SQL Server
docker exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong!Passw0rd' -Q "SELECT 1"

# Check network
docker network inspect contoso-network
```

#### Kubernetes pod crashes
```bash
# Check pod status
kubectl describe pod <pod-name> -n contosouniversity

# View events
kubectl get events -n contosouniversity

# Check previous logs
kubectl logs <pod-name> --previous -n contosouniversity
```

### Debug Mode

Enable debug logging:
```bash
docker run -e ASPNETCORE_ENVIRONMENT=Development \
  -e Logging__LogLevel__Default=Debug \
  contosouniversity:latest
```

---

## ?? Documentation

- **[Containerization Guide](.github/container/CONTAINERIZATION-GUIDE.md)** - Complete containerization documentation
- **[Kubernetes Guide](kubernetes/README.md)** - Kubernetes-specific documentation
- **[Azure Guide](azure/README.md)** - Azure deployment guides

---

## ?? Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test with Docker Compose
5. Submit a pull request

---

## ?? License

This project is licensed under the MIT License.

---

## ?? Support

For issues, questions, or contributions:
- Open an issue on GitHub
- Check the [Troubleshooting](#troubleshooting) section
- Review the [Containerization Guide](.github/container/CONTAINERIZATION-GUIDE.md)

---

## ?? Roadmap

- [ ] Helm charts for Kubernetes
- [ ] Service mesh integration (Istio/Linkerd)
- [ ] Redis caching layer
- [ ] CDN integration
- [ ] Multi-region deployment
- [ ] Disaster recovery automation

---

**Built with ?? using .NET 9.0 | Docker | Kubernetes | Azure**
