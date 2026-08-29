# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
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
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Expose standard ASP.NET Core container ports
EXPOSE 8080
EXPOSE 8081

# Core ASP.NET Core Environment Configuration
# Note: Sensitive secrets (DB connection, JWT Secret) are injected at runtime via Environment Variables in your hosting provider
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080

# Copy published artifacts from build stage
COPY --from=build /app/publish .

# Ensure wwwroot and uploads directory exist
RUN mkdir -p /app/wwwroot/uploads

# Run the ASP.NET Core API
ENTRYPOINT ["dotnet", "BlogManagement.dll"]
