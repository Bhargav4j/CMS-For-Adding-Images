# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

# Copy solution and project files for dependency caching
COPY WebGallery.sln ./
COPY src/WebGallery.Web/WebGallery.Web.csproj src/WebGallery.Web/
COPY src/WebGallery.Application/WebGallery.Application.csproj src/WebGallery.Application/
COPY src/WebGallery.Domain/WebGallery.Domain.csproj src/WebGallery.Domain/
COPY src/WebGallery.Infrastructure/WebGallery.Infrastructure.csproj src/WebGallery.Infrastructure/
COPY tests/WebGallery.Web.Tests/WebGallery.Web.Tests.csproj tests/WebGallery.Web.Tests/
COPY tests/WebGallery.Application.Tests/WebGallery.Application.Tests.csproj tests/WebGallery.Application.Tests/
COPY tests/WebGallery.Domain.Tests/WebGallery.Domain.Tests.csproj tests/WebGallery.Domain.Tests/
COPY tests/WebGallery.Infrastructure.Tests/WebGallery.Infrastructure.Tests.csproj tests/WebGallery.Infrastructure.Tests/

# Restore dependencies
RUN dotnet restore src/WebGallery.Web/WebGallery.Web.csproj

# Copy remaining source code
COPY src/ src/
COPY tests/ tests/

# Build the application
WORKDIR /src/src/WebGallery.Web
RUN dotnet build WebGallery.Web.csproj -c Release --no-restore

# Publish the application
RUN dotnet publish WebGallery.Web.csproj -c Release -o /app/publish --no-build

# Stage 2: Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime

WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application from builder stage
COPY --from=builder /app/publish .

# Set ownership to non-root user
RUN chown -R appuser:appuser /app

# Create directories for logs and uploads with proper permissions
RUN mkdir -p /app/logs /app/uploads && \
    chown -R appuser:appuser /app/logs /app/uploads

# Switch to non-root user
USER appuser

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Expose application port
EXPOSE 8080

# Configure entry point
ENTRYPOINT ["dotnet", "WebGallery.Web.dll"]