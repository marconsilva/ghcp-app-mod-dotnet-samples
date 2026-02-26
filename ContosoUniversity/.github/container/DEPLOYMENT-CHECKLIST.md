# ?? ContosoUniversity Deployment Checklist

## Pre-Deployment Checklist

Use this checklist before deploying to any environment.

---

## ?? Configuration

### 1. Environment Variables ?
- [ ] `ASPNETCORE_ENVIRONMENT` set correctly (Development/Staging/Production)
- [ ] `ConnectionStrings__DefaultConnection` configured with correct database
- [ ] `ASPNETCORE_URLS` set to expected binding (default: http://+:8080)
- [ ] All required secrets created in Kubernetes/Azure
- [ ] No hardcoded secrets in code or config files

### 2. Database Configuration ?
- [ ] Database server is accessible from containers
- [ ] Firewall rules allow container subnet
- [ ] Connection string tested and validated
- [ ] Database migrations are up to date
- [ ] Database user has correct permissions
- [ ] Connection pooling configured (default is fine)
- [ ] Backup strategy in place

### 3. Storage Configuration ?
- [ ] Persistent volume claims created
- [ ] Storage class configured (azure-files, etc.)
- [ ] Upload directory exists and is writable
- [ ] File permissions set correctly (owned by appuser)
- [ ] Storage capacity planned (start with 10Gi for uploads)

---

## ?? Docker Configuration

### 4. Docker Image ?
- [ ] Dockerfile builds successfully
- [ ] Image size is optimized (<300 MB)
- [ ] Base image is up-to-date (mcr.microsoft.com/dotnet/aspnet:9.0)
- [ ] Multi-stage build working correctly
- [ ] .dockerignore configured properly
- [ ] No test projects included in final image
- [ ] Health check command works

### 5. Container Registry ?
- [ ] Registry created (ACR, Docker Hub, ghcr.io)
- [ ] Authentication configured
- [ ] Image push tested
- [ ] Image pull secrets created in Kubernetes (if needed)
- [ ] Retention policy configured
- [ ] Vulnerability scanning enabled (optional)

---

## ?? Kubernetes Configuration

### 6. Kubernetes Manifests ?
- [ ] Namespace created
- [ ] Deployment manifest updated with correct image
- [ ] Service type configured (LoadBalancer/ClusterIP)
- [ ] ConfigMap created with configuration
- [ ] Secrets created with sensitive data
- [ ] Resource requests/limits set appropriately
- [ ] Health probes configured (liveness, readiness, startup)
- [ ] Volume mounts configured correctly

### 7. Auto-Scaling ?
- [ ] HorizontalPodAutoscaler configured
- [ ] Min replicas set (recommended: 3 for HA)
- [ ] Max replicas set (recommended: 10)
- [ ] CPU threshold set (recommended: 70%)
- [ ] Memory threshold set (recommended: 80%)
- [ ] Metrics server installed in cluster

### 8. Ingress Configuration ?
- [ ] Ingress controller installed (NGINX, Traefik, etc.)
- [ ] Ingress manifest configured with correct host
- [ ] DNS records point to load balancer IP
- [ ] SSL/TLS certificate configured
- [ ] cert-manager installed (if using Let's Encrypt)
- [ ] Rate limiting configured (optional)
- [ ] CORS configured if needed

---

## ?? Azure-Specific Configuration

### 9. Azure Resources ?
- [ ] Resource group created
- [ ] Azure Container Registry (ACR) created
- [ ] Azure SQL Database created
- [ ] Storage Account created with file share
- [ ] Log Analytics workspace created
- [ ] Application Insights resource created (optional)
- [ ] All resources in same region for best performance

### 10. Azure Container Apps ?
- [ ] Container Apps environment created
- [ ] Managed identity configured
- [ ] VNet integration configured (if needed)
- [ ] Scale rules configured
- [ ] Secrets configured
- [ ] Custom domain configured (optional)
- [ ] SSL certificate bound

### 11. Azure Kubernetes Service (AKS) ?
- [ ] AKS cluster created
- [ ] Node pool configured (3-10 nodes)
- [ ] Cluster autoscaler enabled
- [ ] ACR integration enabled
- [ ] Managed identity enabled
- [ ] Azure Monitor add-on enabled
- [ ] Network policies configured (optional)
- [ ] Azure Key Vault integration (optional)

---

## ?? Security Configuration

### 12. Security Hardening ?
- [ ] Container runs as non-root user
- [ ] Secrets not in image layers
- [ ] TLS/SSL configured for external traffic
- [ ] Network policies applied (optional)
- [ ] Pod security standards enabled
- [ ] RBAC configured correctly
- [ ] Service accounts with minimal permissions
- [ ] Image vulnerability scanning scheduled

### 13. Secrets Management ?
- [ ] Database passwords in Kubernetes Secrets
- [ ] API keys in Azure Key Vault or K8s Secrets
- [ ] Certificate passwords secured
- [ ] No secrets in git repository
- [ ] Secrets rotated regularly (plan in place)

---

## ?? Monitoring & Observability

### 14. Health Checks ?
- [ ] `/health` endpoint returns 200 OK
- [ ] `/health/live` returns 200 OK
- [ ] `/health/ready` returns 200 OK and validates DB
- [ ] `/health/startup` returns 200 OK
- [ ] Health check intervals configured appropriately
- [ ] Failure thresholds set correctly

### 15. Logging ?
- [ ] Application logs to stdout/stderr
- [ ] Log aggregation configured (Log Analytics)
- [ ] Log retention policy set
- [ ] Log queries tested
- [ ] Sensitive data not logged

### 16. Monitoring ?
- [ ] Application Insights configured (Azure)
- [ ] Azure Monitor dashboards created
- [ ] Key metrics tracked (CPU, memory, requests, errors)
- [ ] Alerts configured for critical conditions
- [ ] On-call rotation notified of alerts

---

## ?? Testing

### 17. Pre-Deployment Testing ?
- [ ] Application builds without errors
- [ ] All unit tests pass (115 tests)
- [ ] Integration tests pass
- [ ] Docker image builds successfully
- [ ] Container starts and serves traffic locally
- [ ] Health endpoints work in container
- [ ] Database connection works from container
- [ ] File uploads work in container

### 18. Post-Deployment Testing ?
- [ ] Application is accessible via ingress
- [ ] Health endpoints return 200 OK
- [ ] Database connectivity verified
- [ ] File uploads work
- [ ] Authentication/authorization works (if configured)
- [ ] Performance is acceptable
- [ ] Auto-scaling works as expected
- [ ] Logs are being collected

### 19. Load Testing (Optional) ?
- [ ] Load testing tool configured (k6, JMeter, etc.)
- [ ] Baseline performance established
- [ ] Auto-scaling validated under load
- [ ] Database performance acceptable
- [ ] No memory leaks detected
- [ ] Response times within SLA

---

## ?? CI/CD Pipeline

### 20. Pipeline Configuration ?
- [ ] GitHub Actions or Azure Pipelines configured
- [ ] Service connections set up
- [ ] Environment secrets configured
- [ ] Build triggers configured (push to main/develop)
- [ ] Test stage passes
- [ ] Docker build stage works
- [ ] Deployment stage configured per environment
- [ ] Manual approvals for production (optional)

### 21. Deployment Strategy ?
- [ ] Rolling update configured (recommended)
- [ ] maxUnavailable and maxSurge set
- [ ] Rollback strategy documented
- [ ] Blue-green deployment configured (optional)
- [ ] Canary deployment configured (optional)

---

## ?? Network Configuration

### 22. Networking ?
- [ ] Ingress controller installed
- [ ] DNS records configured
- [ ] SSL/TLS certificate installed
- [ ] Firewall rules configured
- [ ] Network policies applied (optional)
- [ ] Service mesh installed (optional)

### 23. Connectivity Testing ?
- [ ] External access works (via ingress)
- [ ] Internal service communication works
- [ ] Database connectivity from pods
- [ ] File share mounted correctly
- [ ] Outbound internet access (if needed)

---

## ?? Documentation

### 24. Documentation Complete ?
- [ ] README.md updated with container instructions
- [ ] Deployment guide available
- [ ] Troubleshooting guide created
- [ ] Architecture diagrams included
- [ ] Runbook for operations team
- [ ] Disaster recovery procedures documented

---

## ?? Go-Live Checklist

### Production Deployment

#### Day Before
- [ ] Communicate deployment window to stakeholders
- [ ] Freeze code changes
- [ ] Run full test suite
- [ ] Build and scan production image
- [ ] Prepare rollback plan
- [ ] Schedule maintenance window (if needed)

#### Day Of
- [ ] Verify backup of current production
- [ ] Deploy to production
- [ ] Monitor logs during rollout
- [ ] Verify health checks pass
- [ ] Run smoke tests
- [ ] Verify key functionality
- [ ] Monitor for 30-60 minutes
- [ ] Communicate successful deployment

#### Post-Deployment
- [ ] Monitor metrics for 24-48 hours
- [ ] Review logs for errors
- [ ] Check performance against baseline
- [ ] Verify auto-scaling works
- [ ] Document any issues
- [ ] Update runbook if needed

---

## ?? Common Issues to Verify

### Before You Deploy

1. **Image Size** - Keep under 500 MB
   ```bash
   docker images | grep contosouniversity
   ```

2. **Health Checks** - Must respond quickly
   ```bash
   curl http://localhost:8080/health
   # Should return 200 OK in <50ms
   ```

3. **Database Connection** - Verify from container
   ```bash
   docker exec <container> dotnet ef database get-migrations
   ```

4. **File Permissions** - Non-root user must be able to write
   ```bash
   kubectl exec <pod> -- touch /app/wwwroot/Uploads/test.txt
   ```

5. **Resource Limits** - Not too high or too low
   ```bash
   kubectl top pods -n contosouniversity
   # Should see <50% of limits under normal load
   ```

6. **Auto-Scaling** - HPA must have metrics
   ```bash
   kubectl get hpa -n contosouniversity
   # TARGETS should show percentages, not <unknown>
   ```

---

## ?? Knowledge Transfer

### Team Training Needed

- [ ] Docker basics training
- [ ] Kubernetes fundamentals
- [ ] kubectl commands
- [ ] Troubleshooting containers
- [ ] Reading logs and metrics
- [ ] Deployment process
- [ ] Rollback procedures
- [ ] Incident response

### Runbook Topics

- [ ] How to deploy updates
- [ ] How to scale manually
- [ ] How to view logs
- [ ] How to restart pods
- [ ] How to rollback deployment
- [ ] How to restore from backup
- [ ] Emergency contact list
- [ ] Escalation procedures

---

## ?? Support & Escalation

### L1 Support (Basic Issues)
- Check pod status
- View logs
- Restart pods
- Check health endpoints

### L2 Support (Advanced Issues)
- Investigate application errors
- Database connection issues
- Performance tuning
- Configuration changes

### L3 Support (Critical/Architecture)
- Cluster issues
- Network problems
- Security incidents
- Disaster recovery

---

## ? Sign-Off

### Required Approvals

| Role | Name | Date | Signature |
|:-----|:-----|:-----|:----------|
| Developer | | | |
| DevOps Engineer | | | |
| Database Admin | | | |
| Security Team | | | |
| Product Owner | | | |

---

## ?? Deployment Schedule

### Recommended Timeline

| Phase | Duration | Activities |
|:------|:---------|:-----------|
| **Preparation** | Week 1 | Complete checklist, create resources |
| **Dev Deploy** | Week 2 | Deploy to dev, test thoroughly |
| **Staging Deploy** | Week 3 | Deploy to staging, load testing |
| **Prod Deploy** | Week 4 | Production deployment, monitor |
| **Optimization** | Ongoing | Performance tuning, cost optimization |

---

**Checklist Version**: 1.0  
**Last Updated**: February 26, 2026  
**Status**: Ready for Review
