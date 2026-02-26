# ?? ContosoUniversity Containerization Guide

## Overview

This guide covers the complete containerization of the ContosoUniversity application for cloud-native deployment on Azure, Kubernetes, and other container orchestration platforms.

---

## ?? Table of Contents

1. [Quick Start](#quick-start)
2. [Container Architecture](#container-architecture)
3. [Docker Setup](#docker-setup)
4. [Kubernetes Deployment](#kubernetes-deployment)
5. [Azure Deployment](#azure-deployment)
6. [Configuration](#configuration)
7. [Monitoring & Logging](#monitoring--logging)
8. [Troubleshooting](#troubleshooting)

---

## ?? Quick Start

### Prerequisites
- Docker Desktop or Docker Engine 24.0+
- .NET 9.0 SDK
- Azure CLI (for Azure deployments)
- kubectl (for Kubernetes deployments)
- Helm 3.x (for Kubernetes package management)

### Local Development with Docker Compose

```bash
# Clone the repository
git clone https://github.com/your-org/contosouniversity.git
cd contosouniversity

# Build and run with Docker Compose
docker-compose up -d

# Access the application
open http://localhost:8080

# View logs
docker-compose logs -f web

# Stop containers
docker-compose down
```

### Build Docker Image

```bash
# Build the image
docker build -t contosouniversity:latest .

# Run the container
docker run -d \
  -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=ContosoUniversity;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;" \
  --name contoso-web \
  contosouniversity:latest

# View logs
docker logs -f contoso-web
```

---

## ??? Container Architecture

### Multi-Stage Dockerfile

The Dockerfile uses a multi-stage build process for optimal image size and security:

```
Stage 1: base       - Runtime image (mcr.microsoft.com/dotnet/aspnet:9.0)
Stage 2: build      - Build environment with SDK
Stage 3: publish    - Published application artifacts
Stage 4: final      - Final production image
```

**Key Features:**
- ? Multi-stage build for smaller images
- ? Non-root user for security
- ? Health check endpoints
- ? Proper signal handling
- ? Optimized layer caching

### Image Size Optimization

| Stage | Size |
|:------|:----:|
| SDK Image | ~700 MB |
| Runtime Image | ~210 MB |
| **Final Image** | **~250 MB** |

---

## ?? Docker Setup

### Dockerfile Structure

```dockerfile
# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ContosoUniversity.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER appuser
ENTRYPOINT ["dotnet", "ContosoUniversity.dll"]
```

### Docker Compose Configurations

#### Development (`docker-compose.yml`)
- SQL Server container
- Application container
- Volume mounts for development
- Hot reload support

#### Production (`docker-compose.prod.yml`)
- Resource limits
- Health checks
- Logging configuration
- Multiple replicas
- NGINX reverse proxy

### Environment Variables

| Variable | Description | Default |
|:---------|:------------|:--------|
| `ASPNETCORE_ENVIRONMENT` | Environment name | Production |
| `ASPNETCORE_URLS` | HTTP endpoints | http://+:8080 |
| `ConnectionStrings__DefaultConnection` | Database connection | Required |
| `NotificationQueuePath` | MSMQ queue path | .\\Private$\\ContosoUniversityNotifications |

---

## ?? Kubernetes Deployment

### Quick Deploy to Kubernetes

```bash
# Apply all manifests
kubectl apply -f kubernetes/

# Check deployment status
kubectl get pods -n contosouniversity
kubectl get services -n contosouniversity

# View logs
kubectl logs -f deployment/contoso-web -n contosouniversity

# Access the application
kubectl port-forward service/contoso-web-service 8080:80 -n contosouniversity
```

### Kubernetes Resources

The deployment includes:

1. **Namespace** - `contosouniversity`
2. **ConfigMap** - Application configuration
3. **Secrets** - Sensitive data (connection strings, passwords)
4. **PersistentVolumeClaims** - Storage for SQL and uploads
5. **Deployments**:
   - SQL Server (1 replica)
   - Web Application (3 replicas)
6. **Services**:
   - SQL Server (ClusterIP)
   - Web Application (LoadBalancer)
7. **HorizontalPodAutoscaler** - Auto-scaling 3-10 replicas
8. **Ingress** - External access with SSL/TLS

### Resource Limits

#### Web Application Pods
```yaml
resources:
  requests:
    memory: "512Mi"
    cpu: "250m"
  limits:
    memory: "2Gi"
    cpu: "1000m"
```

#### SQL Server Pod
```yaml
resources:
  requests:
    memory: "2Gi"
    cpu: "1000m"
  limits:
    memory: "4Gi"
    cpu: "2000m"
```

### Health Checks

The application includes three types of health checks:

1. **Liveness Probe** (`/health/live`)
   - Checks if the application is alive
   - Restarts pod if fails

2. **Readiness Probe** (`/health/ready`)
   - Checks if app is ready to serve traffic
   - Includes database connectivity check
   - Removes from load balancer if fails

3. **Startup Probe** (`/health/startup`)
   - Checks if app has started successfully
   - Prevents premature liveness checks

### Auto-Scaling

Horizontal Pod Autoscaler (HPA) configuration:
- **Min Replicas**: 3
- **Max Replicas**: 10
- **CPU Target**: 70%
- **Memory Target**: 80%

```bash
# Check autoscaler status
kubectl get hpa -n contosouniversity

# View scaling events
kubectl describe hpa contoso-web-hpa -n contosouniversity
```

---

## ?? Azure Deployment

### Azure Container Apps

Simple, serverless container deployment:

```bash
# Run deployment script
cd azure
chmod +x deploy-container-apps.sh
./deploy-container-apps.sh
```

**Features:**
- Automatic scaling
- Integrated logging
- Built-in load balancing
- HTTPS endpoints
- Azure SQL integration

### Azure Kubernetes Service (AKS)

Full Kubernetes cluster deployment:

```bash
# Run AKS deployment script
cd azure
chmod +x deploy-aks.sh
./deploy-aks.sh
```

**Includes:**
- AKS cluster (3-10 nodes with autoscaling)
- Azure Container Registry
- Azure SQL Database
- Azure Files for persistent storage
- NGINX Ingress Controller
- cert-manager for SSL certificates
- Azure Monitor integration

### Azure Resources Created

| Resource | Purpose | SKU/Tier |
|:---------|:--------|:---------|
| Container Registry | Docker images | Premium |
| AKS Cluster | Container orchestration | Standard_D4s_v3 |
| SQL Database | Data storage | S2 (Standard) |
| Storage Account | File uploads | Standard_LRS |
| Log Analytics | Monitoring | Per GB |

### Cost Estimation (Monthly)

| Component | Estimated Cost |
|:----------|:--------------:|
| AKS (3 nodes) | ~$400 |
| SQL Database (S2) | ~$150 |
| Storage Account | ~$20 |
| Container Registry | ~$5 |
| Log Analytics | ~$30 |
| **Total** | **~$605/month** |

*Costs vary by region and usage

---

## ?? Configuration

### Connection Strings

#### Development
```
Server=sqlserver;Database=ContosoUniversity;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;
```

#### Azure SQL
```
Server=tcp:contoso-sqlserver.database.windows.net,1433;Database=ContosoUniversity;User ID=sqladmin;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

### Kubernetes Secrets

```bash
# Create secrets
kubectl create secret generic contoso-secrets \
  --from-literal=sql-connection-string="<connection-string>" \
  --from-literal=sql-sa-password="<password>" \
  --namespace contosouniversity

# View secrets
kubectl get secrets -n contosouniversity
kubectl describe secret contoso-secrets -n contosouniversity
```

### ConfigMap

```bash
# View configuration
kubectl get configmap contoso-config -n contosouniversity -o yaml

# Edit configuration
kubectl edit configmap contoso-config -n contosouniversity
```

### Environment-Specific Configuration

| Environment | Database | Replicas | Resources |
|:------------|:---------|:--------:|:----------|
| Development | Docker SQL | 1 | Minimal |
| Staging | Azure SQL | 2 | Medium |
| Production | Azure SQL | 3-10 | Full |

---

## ?? Monitoring & Logging

### Health Check Endpoints

1. **Basic Health** - `GET /health`
   ```bash
   curl http://localhost:8080/health
   ```

2. **Readiness** - `GET /health/ready`
   ```bash
   curl http://localhost:8080/health/ready
   ```

3. **Liveness** - `GET /health/live`
   ```bash
   curl http://localhost:8080/health/live
   ```

4. **Startup** - `GET /health/startup`
   ```bash
   curl http://localhost:8080/health/startup
   ```

### Container Logs

#### Docker
```bash
# View logs
docker logs contoso-web

# Follow logs
docker logs -f contoso-web

# Last 100 lines
docker logs --tail 100 contoso-web
```

#### Kubernetes
```bash
# View pod logs
kubectl logs deployment/contoso-web -n contosouniversity

# Follow logs
kubectl logs -f deployment/contoso-web -n contosouniversity

# Previous container logs (if crashed)
kubectl logs deployment/contoso-web -n contosouniversity --previous

# All pods logs
kubectl logs -l app=contoso-web -n contosouniversity
```

### Azure Monitor

The deployment includes Azure Monitor integration:
- Application Insights for APM
- Log Analytics for centralized logging
- Metrics for performance monitoring
- Alerts for critical events

```bash
# View logs in Azure
az monitor log-analytics workspace show \
  --resource-group contoso-rg \
  --workspace-name contoso-logs
```

---

## ?? Troubleshooting

### Common Issues

#### 1. Container Won't Start

**Symptom**: Container exits immediately

**Check**:
```bash
# View exit code
docker ps -a

# Check logs
docker logs contoso-web

# Common causes:
# - Missing environment variables
# - Database connection failure
# - Port already in use
```

**Solution**:
```bash
# Verify environment variables
docker inspect contoso-web | grep -A 20 Env

# Check port availability
netstat -an | grep 8080

# Test database connection
docker exec contoso-web dotnet --info
```

#### 2. Database Connection Failures

**Symptom**: Application starts but can't connect to database

**Check**:
```bash
# Test SQL Server container
docker exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong!Passw0rd -Q "SELECT 1"

# Check network connectivity
docker exec contoso-web ping sqlserver
```

**Solution**:
```bash
# Ensure containers are on same network
docker network ls
docker network inspect contoso-network

# Verify connection string
kubectl get secret contoso-secrets -n contosouniversity -o yaml
```

#### 3. Pod CrashLoopBackOff

**Symptom**: Kubernetes pod keeps restarting

**Check**:
```bash
# View pod status
kubectl describe pod <pod-name> -n contosouniversity

# Check events
kubectl get events -n contosouniversity --sort-by='.lastTimestamp'

# View previous logs
kubectl logs <pod-name> -n contosouniversity --previous
```

**Common Causes**:
- Failed health checks
- Out of memory
- Application crash
- Missing dependencies

#### 4. File Upload Issues

**Symptom**: File uploads fail or files are lost

**Check**:
```bash
# Verify volume mounts
kubectl describe pod <pod-name> -n contosouniversity | grep -A 10 Mounts

# Check permissions
kubectl exec <pod-name> -n contosouniversity -- ls -la /app/wwwroot/Uploads
```

**Solution**:
```bash
# Ensure PVC is bound
kubectl get pvc -n contosouniversity

# Check storage class
kubectl get storageclass

# Fix permissions
kubectl exec <pod-name> -n contosouniversity -- \
  chown -R appuser:appuser /app/wwwroot/Uploads
```

#### 5. High Memory Usage

**Symptom**: Pods are killed due to OOM

**Check**:
```bash
# View resource usage
kubectl top pods -n contosouniversity

# Check limits
kubectl describe deployment contoso-web -n contosouniversity | grep -A 10 Limits
```

**Solution**:
- Increase memory limits in deployment.yaml
- Optimize application code
- Add more replicas to distribute load

### Debug Commands

```bash
# Get shell in running container
kubectl exec -it <pod-name> -n contosouniversity -- /bin/bash

# Run diagnostic commands
kubectl exec <pod-name> -n contosouniversity -- dotnet --info
kubectl exec <pod-name> -n contosouniversity -- env
kubectl exec <pod-name> -n contosouniversity -- ps aux

# Port forward for local access
kubectl port-forward <pod-name> 8080:8080 -n contosouniversity

# Copy files from pod
kubectl cp <pod-name>:/app/logs ./local-logs -n contosouniversity

# View pod events
kubectl get events --field-selector involvedObject.name=<pod-name> -n contosouniversity
```

---

## ?? Additional Resources

### Documentation
- [Docker Documentation](https://docs.docker.com/)
- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/)
- [Azure Kubernetes Service](https://learn.microsoft.com/en-us/azure/aks/)

### Best Practices
- [12-Factor App](https://12factor.net/)
- [Container Security](https://cheatsheetseries.owasp.org/cheatsheets/Docker_Security_Cheat_Sheet.html)
- [Kubernetes Best Practices](https://kubernetes.io/docs/concepts/configuration/overview/)

### Tools
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [kubectl](https://kubernetes.io/docs/tasks/tools/)
- [Helm](https://helm.sh/)
- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/)

---

## ?? Next Steps

1. **Security Hardening**
   - Implement network policies
   - Enable Pod Security Standards
   - Set up Azure Key Vault integration
   - Configure Azure AD authentication

2. **Performance Optimization**
   - Implement caching (Redis)
   - Add CDN for static files
   - Optimize database queries
   - Enable response compression

3. **Observability**
   - Set up distributed tracing
   - Implement custom metrics
   - Configure alerts and dashboards
   - Add synthetic monitoring

4. **Disaster Recovery**
   - Implement database backups
   - Configure geo-replication
   - Set up failover procedures
   - Test recovery scenarios

---

**Version**: 1.0  
**Last Updated**: February 26, 2026  
**Target Platform**: .NET 9.0 | Docker | Kubernetes | Azure
