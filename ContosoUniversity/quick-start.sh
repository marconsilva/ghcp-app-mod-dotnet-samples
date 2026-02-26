#!/bin/bash

# ContosoUniversity - Quick Start Script
# This script helps you quickly get the application running with Docker

set -e

echo "============================================"
echo "ContosoUniversity Quick Start"
echo "============================================"
echo ""

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Check prerequisites
echo "Checking prerequisites..."

if ! command -v docker &> /dev/null; then
    echo -e "${RED}Error: Docker is not installed${NC}"
    echo "Please install Docker Desktop from https://www.docker.com/products/docker-desktop"
    exit 1
fi

if ! command -v docker-compose &> /dev/null; then
    echo -e "${RED}Error: Docker Compose is not installed${NC}"
    echo "Please install Docker Compose"
    exit 1
fi

echo -e "${GREEN}? Docker is installed${NC}"
echo -e "${GREEN}? Docker Compose is installed${NC}"
echo ""

# Check if Docker daemon is running
if ! docker info &> /dev/null; then
    echo -e "${RED}Error: Docker daemon is not running${NC}"
    echo "Please start Docker Desktop"
    exit 1
fi

echo -e "${GREEN}? Docker daemon is running${NC}"
echo ""

# Clean up any existing containers
echo "Cleaning up existing containers..."
docker-compose down -v 2>/dev/null || true
echo -e "${GREEN}? Cleanup complete${NC}"
echo ""

# Build and start containers
echo "Building and starting containers..."
echo "This may take a few minutes on first run..."
echo ""

docker-compose up -d --build

# Wait for SQL Server to be ready
echo ""
echo "Waiting for SQL Server to be ready..."
for i in {1..30}; do
    if docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' -Q "SELECT 1" &> /dev/null; then
        echo -e "${GREEN}? SQL Server is ready${NC}"
        break
    fi
    if [ $i -eq 30 ]; then
        echo -e "${RED}Error: SQL Server failed to start${NC}"
        docker-compose logs sqlserver
        exit 1
    fi
    echo -n "."
    sleep 2
done

# Wait for web application to be ready
echo ""
echo "Waiting for web application to be ready..."
for i in {1..60}; do
    if curl -f http://localhost:8080/health &> /dev/null; then
        echo -e "${GREEN}? Web application is ready${NC}"
        break
    fi
    if [ $i -eq 60 ]; then
        echo -e "${RED}Error: Web application failed to start${NC}"
        docker-compose logs web
        exit 1
    fi
    echo -n "."
    sleep 2
done

echo ""
echo "============================================"
echo -e "${GREEN}? ContosoUniversity is running!${NC}"
echo "============================================"
echo ""
echo "Application URL: http://localhost:8080"
echo "Health Check:    http://localhost:8080/health"
echo ""
echo "Useful commands:"
echo "  View logs:        docker-compose logs -f"
echo "  Stop application: docker-compose down"
echo "  Restart:          docker-compose restart"
echo "  Clean up:         docker-compose down -v"
echo ""

# Open browser (optional)
if command -v xdg-open &> /dev/null; then
    xdg-open http://localhost:8080 &
elif command -v open &> /dev/null; then
    open http://localhost:8080 &
fi
