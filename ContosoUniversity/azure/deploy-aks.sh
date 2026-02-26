# Azure Kubernetes Service (AKS) deployment script for ContosoUniversity

#!/bin/bash

# Variables
RESOURCE_GROUP="contoso-aks-rg"
LOCATION="eastus"
AKS_NAME="contoso-aks"
ACR_NAME="contosoacr"
SQL_SERVER_NAME="contoso-sqlserver"
SQL_DB_NAME="ContosoUniversity"
STORAGE_ACCOUNT="contosostorage"

echo "Creating resource group..."
az group create --name $RESOURCE_GROUP --location $LOCATION

echo "Creating Azure Container Registry..."
az acr create \
  --resource-group $RESOURCE_GROUP \
  --name $ACR_NAME \
  --sku Premium \
  --admin-enabled true

echo "Creating AKS cluster..."
az aks create \
  --resource-group $RESOURCE_GROUP \
  --name $AKS_NAME \
  --node-count 3 \
  --node-vm-size Standard_D4s_v3 \
  --enable-addons monitoring \
  --generate-ssh-keys \
  --attach-acr $ACR_NAME \
  --network-plugin azure \
  --enable-managed-identity \
  --enable-cluster-autoscaler \
  --min-count 3 \
  --max-count 10

echo "Getting AKS credentials..."
az aks get-credentials \
  --resource-group $RESOURCE_GROUP \
  --name $AKS_NAME \
  --overwrite-existing

echo "Creating Azure SQL Database..."
az sql server create \
  --name $SQL_SERVER_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user sqladmin \
  --admin-password 'YourStrong!Passw0rd'

az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER_NAME \
  --name $SQL_DB_NAME \
  --service-objective S2

# Allow AKS to access SQL
AKS_SUBNET_ID=$(az aks show --resource-group $RESOURCE_GROUP --name $AKS_NAME --query "agentPoolProfiles[0].vnetSubnetId" -o tsv)
az sql server vnet-rule create \
  --server $SQL_SERVER_NAME \
  --resource-group $RESOURCE_GROUP \
  --name aks-subnet-rule \
  --subnet $AKS_SUBNET_ID

echo "Creating Azure Storage Account..."
az storage account create \
  --name $STORAGE_ACCOUNT \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku Standard_LRS \
  --kind StorageV2

# Create file share for uploads
STORAGE_KEY=$(az storage account keys list \
  --resource-group $RESOURCE_GROUP \
  --account-name $STORAGE_ACCOUNT \
  --query "[0].value" -o tsv)

az storage share create \
  --name uploads \
  --account-name $STORAGE_ACCOUNT \
  --account-key $STORAGE_KEY

echo "Building and pushing Docker image..."
az acr build \
  --registry $ACR_NAME \
  --image contosouniversity:latest \
  --image contosouniversity:$(git rev-parse --short HEAD) \
  --file ../Dockerfile \
  ..

echo "Installing NGINX Ingress Controller..."
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update
helm install nginx-ingress ingress-nginx/ingress-nginx \
  --namespace ingress-nginx \
  --create-namespace \
  --set controller.replicaCount=2 \
  --set controller.nodeSelector."kubernetes\.io/os"=linux \
  --set controller.service.externalTrafficPolicy=Local

echo "Installing cert-manager for SSL certificates..."
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.0/cert-manager.yaml

echo "Waiting for cert-manager to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/cert-manager -n cert-manager
kubectl wait --for=condition=available --timeout=300s deployment/cert-manager-webhook -n cert-manager

echo "Creating Kubernetes secrets..."
CONNECTION_STRING="Server=tcp:$SQL_SERVER_NAME.database.windows.net,1433;Database=$SQL_DB_NAME;User ID=sqladmin;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

kubectl create namespace contosouniversity

kubectl create secret generic contoso-secrets \
  --from-literal=sql-connection-string="$CONNECTION_STRING" \
  --from-literal=sql-sa-password='YourStrong!Passw0rd' \
  --namespace contosouniversity

# Create secret for Azure Files
kubectl create secret generic azure-files-secret \
  --from-literal=azurestorageaccountname=$STORAGE_ACCOUNT \
  --from-literal=azurestorageaccountkey=$STORAGE_KEY \
  --namespace contosouniversity

echo "Updating Kubernetes manifests..."
# Update image in deployment.yaml
sed -i "s|<your-registry>|$ACR_NAME.azurecr.io|g" ../kubernetes/deployment.yaml

echo "Deploying application to AKS..."
kubectl apply -f ../kubernetes/deployment.yaml
kubectl apply -f ../kubernetes/ingress.yaml

echo "Waiting for deployment to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/contoso-web -n contosouniversity

echo "Getting application URL..."
INGRESS_IP=$(kubectl get service nginx-ingress-ingress-nginx-controller \
  -n ingress-nginx \
  -o jsonpath='{.status.loadBalancer.ingress[0].ip}')

echo ""
echo "======================================"
echo "Deployment completed successfully!"
echo "======================================"
echo ""
echo "Application URL: http://$INGRESS_IP"
echo "Note: Configure DNS to point your domain to $INGRESS_IP"
echo ""
echo "Useful commands:"
echo "  kubectl get pods -n contosouniversity"
echo "  kubectl logs -f deployment/contoso-web -n contosouniversity"
echo "  kubectl describe pod <pod-name> -n contosouniversity"
echo "  kubectl exec -it <pod-name> -n contosouniversity -- /bin/bash"
