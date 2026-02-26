# Containerization Strategy for ContosoUniversity

## Multi-Stage Dockerfile for .NET 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore dependencies
COPY ["ContosoUniversity.csproj", "./"]
RUN dotnet restore "ContosoUniversity.csproj"

# Copy all source files
COPY . .

# Build the application
RUN dotnet build "ContosoUniversity.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ContosoUniversity.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && \
    chown -R appuser:appuser /app

# Copy published application
COPY --from=publish /app/publish .

# Create directories for uploads and ensure permissions
RUN mkdir -p /app/wwwroot/Uploads/TeachingMaterials && \
    chown -R appuser:appuser /app/wwwroot

# Switch to non-root user
USER appuser

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ContosoUniversity.dll"]
