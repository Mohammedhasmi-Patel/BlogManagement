# Complete Vercel Deployment Plan (.NET 10 + PostgreSQL)

This document provides a step-by-step guide to hosting and deploying your **ASP.NET Core 10 Web API** containerized on **Vercel** with **Neon PostgreSQL**.

---

## 🏗️ Architecture Overview

* **Backend Runtime:** ASP.NET Core 10 Web API running in an OCI-compliant Linux container.
* **Container Build:** Vercel Container Registry (VCR) using [`Dockerfile.vercel`](file:///e:/AspnetCore/BlogManagement/Dockerfile.vercel).
* **Execution:** Vercel Fluid Compute (autoscaling with active CPU compute).
* **Database:** Neon Serverless PostgreSQL (`ep-dry-fog-a8712p5i-pooler.us-east-2.aws.neon.tech`).

---

## 📋 Pre-Deployment Checklist

- [x] Converted database provider from SQL Server to PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`).
- [x] Generated fresh initial PostgreSQL EF Core migrations.
- [x] Multi-stage `Dockerfile` and `Dockerfile.vercel` configured for .NET 10.
- [x] Configured `.dockerignore` to exclude secrets and build artifacts.
- [x] Codebase builds cleanly (`dotnet build -c Release` with 0 warnings / 0 errors).

---

## 🚀 Step-by-Step Deployment Instructions

### Step 1: Apply Initial Migration to Neon Database (One-time)

Run EF Core migration from your terminal to create all tables on your remote Neon database:

```bash
dotnet ef database update --project BlogManagement --connection "Host=ep-dry-fog-a8712p5i-pooler.us-east-2.aws.neon.tech;Database=blog-management;Username=neondb_owner;Password=YOUR_ACTUAL_NEON_PASSWORD;SSL Mode=Require;Trust Server Certificate=true;"
```

---

### Step 2: Commit & Push Code to GitHub

```bash
git add .
git commit -m "feat: configure PostgreSQL and Dockerfile for Vercel container deployment"
git push origin main
```

---

### Step 3: Import Project in Vercel

1. Log in to [Vercel](https://vercel.com).
2. Click **"Add New..."** $\rightarrow$ **"Project"**.
3. Select your GitHub repository (`BlogManagement`).
4. **Framework Preset:** Leave as *Other* (Vercel will auto-detect `Dockerfile.vercel`).
5. **Root Directory:** `./` (Leave as default).

---

### Step 4: Configure Environment Variables in Vercel

Before hitting **Deploy**, expand the **Environment Variables** section and add the following:

| Variable Key | Example Value | Description |
| :--- | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Runs in Production mode |
| `ASPNETCORE_HTTP_PORTS` | `8080` | Container port |
| `ConnectionStrings__DefaultConnection` | `Host=ep-dry-fog-a8712p5i-pooler.us-east-2.aws.neon.tech;Database=blog-management;Username=neondb_owner;Password=YOUR_NEON_PASSWORD;SSL Mode=Require;Trust Server Certificate=true;` | Neon PostgreSQL connection string |
| `JwtConfiguration__SecretKey` | `mPyFCZrqdGJ8u/A5zX6aw8lQMxR8YYQcRXsCH7P41ZE=` | JWT signing secret (min 32 chars) |
| `JwtConfiguration__Issuer` | `BlogManagementBackend` | JWT issuer |
| `JwtConfiguration__Audience` | `BlogManagementFrontend` | JWT audience |
| `JwtConfiguration__JwtExpireInMinutes` | `60` | Token expiration time |
| `AppSettings__BaseUrl` | `https://your-backend-project.vercel.app` | Your deployed backend URL |
| `AllowedOrigins__0` | `https://your-frontend.vercel.app` | Allowed CORS frontend URL |
| `AllowedOrigins__1` | `http://localhost:3000` | Local frontend dev testing |

> [!IMPORTANT]
> .NET Core uses **double underscores (`__`)** to map nested configuration keys (e.g. `ConnectionStrings__DefaultConnection`).

---

### Step 5: Deploy & Monitor

1. Click **"Deploy"**.
2. Vercel will:
   * Build the Docker image via multi-stage .NET 10 SDK.
   * Push image to Vercel Container Registry.
   * Deploy the container to Fluid Compute.
3. Once deployed, note down your production URL: `https://your-backend-project.vercel.app`.

---

### Step 6: Post-Deployment Verification

1. Test standard endpoints using Postman / Curl:
   ```bash
   curl -i https://your-backend-project.vercel.app/api/category
   ```
2. Test User Registration / Login:
   ```bash
   POST https://your-backend-project.vercel.app/api/auth/login
   ```
3. Update `AppSettings__BaseUrl` in Vercel Environment Variables with your final Vercel domain if different.

---

## 💡 Best Practices for Production

1. **Secrets:** Never commit `.env` or hardcoded database passwords to Git.
2. **File Storage in Serverless:**
   * Local container storage (`/app/wwwroot/uploads`) is ephemeral on serverless containers (it resets on cold starts or new instance scaling).
   * For persistent media uploads in production, integrating cloud object storage (such as **Vercel Blob**, **Cloudinary**, or **AWS S3**) is recommended.
3. **Database Connection Pooling:**
   * Neon includes a built-in connection pooler (`-pooler` in your hostname). Using this host ensures efficient connection pooling when multiple container instances scale up.
