# ContosoUniversity - Quick Start Script (PowerShell)
# This script helps you quickly get the application running with Docker

$ErrorActionPreference = "Stop"

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "ContosoUniversity Quick Start" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor Yellow

try {
    docker --version | Out-Null
    Write-Host "? Docker is installed" -ForegroundColor Green
}
catch {
    Write-Host "Error: Docker is not installed" -ForegroundColor Red
    Write-Host "Please install Docker Desktop from https://www.docker.com/products/docker-desktop"
    exit 1
}

try {
    docker-compose --version | Out-Null
    Write-Host "? Docker Compose is installed" -ForegroundColor Green
}
catch {
    Write-Host "Error: Docker Compose is not installed" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Check if Docker daemon is running
try {
    docker info | Out-Null
    Write-Host "? Docker daemon is running" -ForegroundColor Green
}
catch {
    Write-Host "Error: Docker daemon is not running" -ForegroundColor Red
    Write-Host "Please start Docker Desktop"
    exit 1
}

Write-Host ""

# Clean up any existing containers
Write-Host "Cleaning up existing containers..." -ForegroundColor Yellow
docker-compose down -v 2>$null
Write-Host "? Cleanup complete" -ForegroundColor Green
Write-Host ""

# Build and start containers
Write-Host "Building and starting containers..." -ForegroundColor Yellow
Write-Host "This may take a few minutes on first run..." -ForegroundColor Yellow
Write-Host ""

docker-compose up -d --build

# Wait for SQL Server to be ready
Write-Host ""
Write-Host "Waiting for SQL Server to be ready..." -ForegroundColor Yellow
$sqlReady = $false
for ($i = 1; $i -le 30; $i++) {
    try {
        docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' -Q "SELECT 1" 2>$null | Out-Null
        Write-Host "? SQL Server is ready" -ForegroundColor Green
        $sqlReady = $true
        break
    }
    catch {
        Write-Host "." -NoNewline
        Start-Sleep -Seconds 2
    }
}

if (-not $sqlReady) {
    Write-Host ""
    Write-Host "Error: SQL Server failed to start" -ForegroundColor Red
    docker-compose logs sqlserver
    exit 1
}

# Wait for web application to be ready
Write-Host ""
Write-Host "Waiting for web application to be ready..." -ForegroundColor Yellow
$webReady = $false
for ($i = 1; $i -le 60; $i++) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:8080/health" -UseBasicParsing -TimeoutSec 2
        if ($response.StatusCode -eq 200) {
            Write-Host "? Web application is ready" -ForegroundColor Green
            $webReady = $true
            break
        }
    }
    catch {
        Write-Host "." -NoNewline
        Start-Sleep -Seconds 2
    }
}

if (-not $webReady) {
    Write-Host ""
    Write-Host "Error: Web application failed to start" -ForegroundColor Red
    docker-compose logs web
    exit 1
}

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "? ContosoUniversity is running!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Application URL: http://localhost:8080" -ForegroundColor White
Write-Host "Health Check:    http://localhost:8080/health" -ForegroundColor White
Write-Host ""
Write-Host "Useful commands:" -ForegroundColor Yellow
Write-Host "  View logs:        docker-compose logs -f" -ForegroundColor White
Write-Host "  Stop application: docker-compose down" -ForegroundColor White
Write-Host "  Restart:          docker-compose restart" -ForegroundColor White
Write-Host "  Clean up:         docker-compose down -v" -ForegroundColor White
Write-Host ""

# Open browser
Start-Process "http://localhost:8080"
