# ?? ContosoUniversity Container Quick Reference Card

## ?? One-Command Start

```powershell
# Windows
.\quick-start.ps1

# Linux/Mac
./quick-start.sh
```

**Opens**: http://localhost:8080

---

## ?? Docker Commands

```bash
# Build image
docker build -t contoso:latest .

# Run container
docker run -d -p 8080:8080 --name contoso contoso:latest

# View logs
docker logs -f contoso

# Stop container
docker stop contoso

# Remove container
docker rm contoso

# Clean up everything
docker system prune -a
```

---

## ?? Docker Compose Commands

```bash
# Start (detached)
docker-compose up -d

# Start (with logs)
docker-compose up

# Stop
docker-compose down

# Stop and remove volumes
docker-compose down -v

# Rebuild and start
docker-compose up -d --build

# View logs
docker-compose logs -f

# View logs for specific service
docker-compose logs -f web

# Restart service
docker-compose restart web

# Scale service
docker-compose up -d --scale web=3
```

---

## ?? Kubernetes Commands

```bash
# Deploy
kubectl apply -f kubernetes/

# Check status
kubectl get all -n contosouniversity

# Get pods
kubectl get pods -n contosouniversity

# View logs
kubectl logs -f deployment/contoso-web -n contosouniversity

# Get services
kubectl get svc -n contosouniversity

# Describe pod
kubectl describe pod <pod-name> -n contosouniversity

# Port forward
kubectl port-forward service/contoso-web-service 8080:80 -n contosouniversity

# Scale manually
kubectl scale deployment/contoso-web --replicas=5 -n contosouniversity

# View HPA status
kubectl get hpa -n contosouniversity

# Delete deployment
kubectl delete -f kubernetes/
```

---

## ?? Azure Commands

### Container Apps
```bash
# Deploy
./azure/deploy-container-apps.sh

# View logs
az containerapp logs tail \
  --name contoso-web \
  --resource-group contoso-rg

# View app URL
az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query properties.configuration.ingress.fqdn

# Scale
az containerapp update \
  --name contoso-web \
  --resource-group contoso-rg \
  --min-replicas 3 \
  --max-replicas 10
```

### AKS
```bash
# Deploy
./azure/deploy-aks.sh

# Get credentials
az aks get-credentials \
  --resource-group contoso-aks-rg \
  --name contoso-aks

# Then use kubectl commands above
```

---

## ?? Health Check Endpoints

```bash
# Basic health
curl http://localhost:8080/health

# Readiness (includes DB check)
curl http://localhost:8080/health/ready

# Liveness
curl http://localhost:8080/health/live

# Startup
curl http://localhost:8080/health/startup
```

**Expected Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2026-02-26T15:30:00Z",
  "version": "1.0.0",
  "environment": "Production"
}
```

---

## ?? Troubleshooting Commands

### Docker
```bash
# Container won't start
docker logs <container-name>
docker inspect <container-name>

# Check running containers
docker ps -a

# Get into container
docker exec -it <container-name> bash

# Check container resources
docker stats
```

### Kubernetes
```bash
# Pod not starting
kubectl describe pod <pod-name> -n contosouniversity
kubectl logs <pod-name> -n contosouniversity
kubectl get events -n contosouniversity --sort-by='.lastTimestamp'

# Previous container logs (if crashed)
kubectl logs <pod-name> -n contosouniversity --previous

# Get shell in pod
kubectl exec -it <pod-name> -n contosouniversity -- /bin/bash

# Check resource usage
kubectl top pods -n contosouniversity
kubectl top nodes

# Check HPA
kubectl describe hpa contoso-web-hpa -n contosouniversity
```

---

## ?? Monitoring Commands

### Resource Usage
```bash
# Docker
docker stats

# Kubernetes
kubectl top pods -n contosouniversity
kubectl top nodes

# Azure Container Apps
az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query properties.template.scale
```

### Logs
```bash
# Docker
docker logs --tail 100 -f <container>

# Kubernetes
kubectl logs -f deployment/contoso-web -n contosouniversity --tail=100

# All pods
kubectl logs -f -l app=contoso-web -n contosouniversity
```

---

## ?? Configuration Commands

### Kubernetes ConfigMap
```bash
# View
kubectl get configmap contoso-config -n contosouniversity -o yaml

# Edit
kubectl edit configmap contoso-config -n contosouniversity

# Delete and recreate
kubectl delete configmap contoso-config -n contosouniversity
kubectl create configmap contoso-config \
  --from-literal=ASPNETCORE_ENVIRONMENT=Production \
  -n contosouniversity
```

### Kubernetes Secrets
```bash
# View (base64 encoded)
kubectl get secret contoso-secrets -n contosouniversity -o yaml

# Decode secret
kubectl get secret contoso-secrets -n contosouniversity -o jsonpath='{.data.sql-connection-string}' | base64 --decode

# Create/update secret
kubectl create secret generic contoso-secrets \
  --from-literal=sql-connection-string="<connection-string>" \
  --namespace contosouniversity \
  --dry-run=client -o yaml | kubectl apply -f -
```

---

## ?? Update & Rollback Commands

### Update Application
```bash
# Build new image
docker build -t contoso:v2 .

# Push to registry
docker push myregistry/contoso:v2

# Update Kubernetes deployment
kubectl set image deployment/contoso-web \
  web=myregistry/contoso:v2 \
  -n contosouniversity

# Watch rollout
kubectl rollout status deployment/contoso-web -n contosouniversity
```

### Rollback
```bash
# View rollout history
kubectl rollout history deployment/contoso-web -n contosouniversity

# Rollback to previous version
kubectl rollout undo deployment/contoso-web -n contosouniversity

# Rollback to specific revision
kubectl rollout undo deployment/contoso-web --to-revision=2 -n contosouniversity
```

---

## ?? Common Tasks

### Scale Application
```bash
# Manual scaling (Kubernetes)
kubectl scale deployment/contoso-web --replicas=5 -n contosouniversity

# Check current scale
kubectl get deployment contoso-web -n contosouniversity
```

### Restart Application
```bash
# Docker
docker restart <container-name>

# Docker Compose
docker-compose restart web

# Kubernetes (rolling restart)
kubectl rollout restart deployment/contoso-web -n contosouniversity
```

### View Application URL
```bash
# Kubernetes (if LoadBalancer)
kubectl get service contoso-web-service -n contosouniversity

# Azure Container Apps
az containerapp show \
  --name contoso-web \
  --resource-group contoso-rg \
  --query properties.configuration.ingress.fqdn \
  --output tsv
```

---

## ?? File Locations

```
?? Root
??? ?? Dockerfile
??? ?? .dockerignore
??? ?? docker-compose.yml
??? ?? docker-compose.prod.yml
??? ?? quick-start.ps1
??? ?? quick-start.sh
??? ?? CONTAINER-README.md
?
??? ?? kubernetes/
?   ??? deployment.yaml
?   ??? ingress.yaml
?
??? ?? azure/
?   ??? deploy-container-apps.sh
?   ??? deploy-aks.sh
?
??? ?? .github/
?   ??? ?? workflows/
?   ?   ??? docker-build.yml
?   ??? ?? container/
?       ??? CONTAINERIZATION-GUIDE.md
?       ??? CONTAINERIZATION-COMPLETE.md
?       ??? ARCHITECTURE-DIAGRAMS.md
?       ??? DEPLOYMENT-CHECKLIST.md
?       ??? EXECUTIVE-SUMMARY.md
?       ??? QUICK-REFERENCE.md ? You are here
?
??? ?? Controllers/
    ??? HealthController.cs
```

---

## ?? Emergency Commands

### Immediate Issues

```bash
# Application down - restart immediately
kubectl rollout restart deployment/contoso-web -n contosouniversity

# Database connection issues - check secret
kubectl get secret contoso-secrets -n contosouniversity -o yaml

# High CPU/Memory - scale up immediately
kubectl scale deployment/contoso-web --replicas=8 -n contosouniversity

# View recent errors
kubectl logs --tail=50 deployment/contoso-web -n contosouniversity | grep -i error

# Check pod health
kubectl get pods -n contosouniversity
kubectl describe pod <pod-name> -n contosouniversity
```

### Rollback Fast

```bash
# Immediate rollback
kubectl rollout undo deployment/contoso-web -n contosouniversity

# Verify rollback
kubectl rollout status deployment/contoso-web -n contosouniversity
```

---

## ?? Quick Help

| Issue | Command |
|:------|:--------|
| **App not responding** | `kubectl logs -f deployment/contoso-web -n contosouniversity` |
| **High CPU** | `kubectl top pods -n contosouniversity` |
| **Can't connect to DB** | `kubectl exec <pod> -n contosouniversity -- curl http://localhost:8080/health/ready` |
| **Need to restart** | `kubectl rollout restart deployment/contoso-web -n contosouniversity` |
| **Scale immediately** | `kubectl scale deployment/contoso-web --replicas=8 -n contosouniversity` |
| **View metrics** | `kubectl get hpa -n contosouniversity` |

---

## ?? Success URLs

| Environment | URL |
|:------------|:----|
| **Local Docker** | http://localhost:8080 |
| **Local K8s (port-forward)** | http://localhost:8080 |
| **Azure Container Apps** | https://contoso-web.<hash>.azurecontainerapps.io |
| **AKS (after DNS)** | https://contosouniversity.example.com |

---

## ? Health Check Quick Test

```bash
# All health checks should return 200 OK
curl -i http://localhost:8080/health
curl -i http://localhost:8080/health/live
curl -i http://localhost:8080/health/ready
curl -i http://localhost:8080/health/startup
```

---

## ?? Demo Commands (5 min)

```bash
# 1. Start locally (1 min)
.\quick-start.ps1

# 2. Check health (30 sec)
curl http://localhost:8080/health

# 3. View containers (30 sec)
docker-compose ps

# 4. View logs (1 min)
docker-compose logs web | Select-Object -Last 20

# 5. Scale (1 min)
kubectl scale deployment/contoso-web --replicas=5 -n contosouniversity
kubectl get pods -n contosouniversity

# 6. Clean up (1 min)
docker-compose down
```

---

**Quick Reference Version**: 1.0  
**Last Updated**: February 26, 2026  
**Print this page for quick access during deployments!** ??
