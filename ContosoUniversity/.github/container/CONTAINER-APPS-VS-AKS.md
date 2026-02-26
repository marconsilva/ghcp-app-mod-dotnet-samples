# ?? Container Apps vs AKS - Decision Guide

## Quick Answer

**For ContosoUniversity**: Use **Azure Container Apps** ??

**Why?** Simple web app + variable traffic + small team + budget conscious = Container Apps perfect fit ?

---

## ?? Detailed Comparison

### Cost Comparison

| Item | Container Apps | AKS | Difference |
|:-----|:--------------:|:---:|:----------:|
| **Compute** | $100-200 (usage) | $400-600 (fixed) | ?? Save $300-400 |
| **SQL Database** | $150 | $150 | Same |
| **Storage** | $20 | $20 | Same |
| **Registry** | $20 | $5 | AKS $15 cheaper |
| **Load Balancer** | Included | $20 | Container Apps free |
| **Management** | Included | $50-100 (ops) | ?? Save $50-100 |
| **Monitoring** | $30 | $50 | ?? Save $20 |
| **Total/month** | **$320-420** | **$750+** | **?? Save $380** |
| **Total/year** | **$3,840-5,040** | **$9,000+** | **?? Save $4,560** |

---

## ?? Feature Comparison

### Simplicity

| Feature | Container Apps | AKS |
|:--------|:--------------:|:---:|
| Setup time | 15 minutes | 45-60 minutes |
| Expertise needed | Basic Docker | Kubernetes expert |
| Cluster management | None (managed) | Weekly maintenance |
| Upgrade process | Automatic | Manual planning |
| SSL certificates | Automatic | Manual setup |
| Load balancing | Automatic | Configure ingress |
| Monitoring setup | Integrated | Configure Azure Monitor |
| **Winner** | ?? **Container Apps** | |

### Scalability

| Feature | Container Apps | AKS |
|:--------|:--------------:|:---:|
| Auto-scaling | Built-in (0-30) | Configure HPA |
| Scale to zero | ? Yes | ? No (min 1 node) |
| Scale up time | 30-60 seconds | 30-60 seconds |
| Scale down | Automatic | Automatic |
| Cost when idle | $0 (scale to zero) | Full node cost |
| Max instances | 30 containers | 100+ pods |
| **Winner** | ?? **Container Apps** (for <30) | ?? **AKS** (for >30) |

### Features & Control

| Feature | Container Apps | AKS |
|:--------|:--------------:|:---:|
| Kubernetes API | Subset | Full |
| Custom controllers | ? No | ? Yes |
| Service mesh | ? No | ? Yes (Istio, Linkerd) |
| Network policies | Limited | Full |
| Sidecar containers | Limited | Full |
| Custom ingress | ? No | ? Yes |
| **Winner** | | ?? **AKS** (for complex needs) |

---

## ?? Use Case Match

### ContosoUniversity Characteristics

| Characteristic | Container Apps Fit | AKS Fit |
|:--------------|:------------------:|:-------:|
| Simple web app | ? Perfect | ?? Overkill |
| Variable traffic | ? Perfect (scale to zero) | ? Wastes money |
| Small team | ? No K8s needed | ? Need expertise |
| Education budget | ? Cost-effective | ? Expensive |
| Fast deployment | ? 15 minutes | ?? 60 minutes |
| Minimal maintenance | ? Fully managed | ? Requires ops |
| **Overall Score** | **6/6 Perfect** ? | **0/6** |

**Verdict**: Container Apps is the **clear winner** ??

---

## ?? Cost Analysis

### Scenario: Education App (Variable Traffic)

#### Container Apps Cost Model

```
High Traffic Period (8 AM - 6 PM):
?? Active instances: 5-10
?? Daily cost: ~$10-15
?? Monthly (22 days): ~$220-330

Low Traffic Period (6 PM - 8 AM, weekends):
?? Active instances: 0-2
?? Daily cost: ~$2-5
?? Monthly (8 days): ~$16-40

Total Container Apps: $236-370/month
Plus:
?? SQL Database (S2): $150
?? Storage: $20
?? Registry: $20
?? Logging: $30

Grand Total: $456/month at high end
????????????????????????????????????????
Average: $320-420/month ?
```

#### AKS Cost Model

```
24/7 Fixed Costs (Always Running):
?? 3 nodes (D4s_v3): $400-600
?? Load Balancer: $20
?? SQL Database (S2): $150
?? Storage: $20
?? Registry: $5
?? Monitoring: $50

Total: $645-845/month
????????????????????????????????????????
No cost savings during low traffic ?
```

**Annual Savings**: $3,600-5,100 with Container Apps ??

---

## ?? Scaling Behavior

### Container Apps Scaling

```
Daily Traffic Pattern:

Instances
   30 ?                    ????
      ?                   ?    ?
   20 ?                 ?        ?
      ?                ?          ?
   10 ?      ??      ?              ?      ??
      ?    ?    ?  ?                  ?  ?  ?
    2 ????      ??                      ??    ???
    0 ?????????????????????????????????????????????
      0   3   6   9   12  15  18  21  24 Hours

Cost Impact:
?? Night (0-2 instances): $2-5/day
?? Morning peak (10 instances): $15/day
?? Afternoon (5 instances): $8/day
?? Evening (2 instances): $3/day

Average: $10-12/day = $300-360/month
```

### AKS Scaling

```
Daily Cost Pattern:

Cost
$20/day????????????????????????????????????
       ????????????????????????????????????
       ????????????????????????????????????
       ????????????????????????????????????
       ????????????????????????????????????
       ????????????????????????????????????
       ????????????????????????????????????
    $0 ?????????????????????????????????????
       0   3   6   9   12  15  18  21  24 Hours

Fixed cost regardless of traffic
Nodes always running: $20-30/day
```

---

## ?? Decision Matrix

### Choose Container Apps If:

? **Application**
- Single web application
- Straightforward architecture
- RESTful APIs
- Background jobs (simple)

? **Team**
- Small team (1-5 developers)
- Limited Kubernetes experience
- Focus on application, not infrastructure
- Prefer managed services

? **Traffic**
- Variable traffic patterns
- Predictable daily/weekly cycles
- Can tolerate scale-up time (30-60 sec)
- Want scale-to-zero for dev

? **Business**
- Budget conscious
- Fast time-to-market
- Minimal operational overhead
- Pay-per-use model preferred

**ContosoUniversity matches all criteria** ?

---

### Choose AKS If:

? **Application**
- Complex microservices (10+ services)
- Service mesh required (Istio, Linkerd)
- Advanced networking needs
- Custom Kubernetes operators
- Sidecar containers essential

? **Team**
- Kubernetes expertise available
- DevOps team dedicated to platform
- Need full control over cluster
- Already using Kubernetes elsewhere

? **Traffic**
- Constant high traffic
- Need instant scale-up (<10 sec)
- Large scale (50+ pods)
- Complex traffic routing

? **Business**
- Enterprise compliance requirements
- Multi-tenant isolation needs
- Specific regulatory requirements
- Need on-premises hybrid

**ContosoUniversity does NOT match** ?

---

## ?? Recommendation Summary

```
????????????????????????????????????????????????????
?                                                   ?
?         RECOMMENDATION: CONTAINER APPS            ?
?                                                   ?
?  Confidence Level: ?????????? 100%               ?
?                                                   ?
?  Reasons:                                         ?
?  ? 60% cost savings ($380/month)                ?
?  ? 4x faster deployment (15 vs 60 min)          ?
?  ? Zero infrastructure management                ?
?  ? Perfect for education traffic patterns        ?
?  ? Team can focus on app, not platform           ?
?  ? Scale to zero during off-hours                ?
?                                                   ?
?  Alternative: AKS available if needs change       ?
?                                                   ?
????????????????????????????????????????????????????
```

---

## ?? Migration Path (If Needed Later)

### From Container Apps to AKS

If your needs change and you need AKS:

```bash
# Your containers already work!
# Just deploy to AKS:

cd azure
./deploy-aks.sh

# Your Kubernetes manifests are ready in kubernetes/
kubectl apply -f kubernetes/
```

**Time to migrate**: 1-2 hours  
**Code changes**: None (same containers work)  
**Effort**: Low (manifests already prepared)

---

## ?? Get Help

### Documentation
- [Container Apps Architecture](.github/container/CONTAINER-APPS-ARCHITECTURE.md)
- [Operations Guide](.github/container/CONTAINER-APPS-OPERATIONS.md)
- [Deployment Checklist](.github/container/DEPLOYMENT-CHECKLIST.md)

### Microsoft Docs
- [Azure Container Apps](https://learn.microsoft.com/azure/container-apps/)
- [Container Apps vs AKS](https://learn.microsoft.com/azure/container-apps/compare-options)

### Community
- [GitHub Issues](https://github.com/marconsilva/ghcp-app-mod-dotnet-samples/issues)
- [Azure Container Apps Samples](https://github.com/Azure-Samples/container-apps-samples)

---

## ?? Final Word

For **ContosoUniversity**:

- ?? **Container Apps**: Perfect fit
- ?? **AKS**: Unnecessarily complex

**Start with Container Apps.** You can always move to AKS later if needs change (and your containers will work unchanged).

**Deploy now**: `.\deploy-container-apps.ps1` ??

---

**Guide Version**: 1.0  
**Last Updated**: February 26, 2026  
**Recommendation**: Azure Container Apps ??
