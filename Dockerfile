# Stage 1: Build stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies (caching layer)
COPY ["BlogManagement/BlogManagement.csproj", "BlogManagement/"]
RUN dotnet restore "BlogManagement/BlogManagement.csproj"

# Copy the rest of the source code
COPY . .

# Build and publish the application
WORKDIR "/src/BlogManagement"
RUN dotnet publish "BlogManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Expose standard container port
EXPOSE 8080

# Environment Configuration for Serverless / Vercel Fluid Compute
# 1. Disable JIT W^X to prevent SIGSEGV (exit code 139) in microVM / hypervisor sandboxes
ENV DOTNET_EnableWriteXorExecute=0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080

# Copy published artifacts from build stage
COPY --from=build /app/publish .

# Ensure wwwroot and uploads directory exist
RUN mkdir -p /app/wwwroot/uploads

# Run the ASP.NET Core API
ENTRYPOINT ["dotnet", "BlogManagement.dll"]

