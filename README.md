# **Expense Tracker Application** 💰

A full-stack web application for tracking personal expenses. The backend is built with **ASP.NET Core 8** and the frontend with **React 19**, communicating via REST API with **JWT-based authentication**.

---

## **Table of Contents**
- [Architecture](#architecture)
- [Backend Setup](#backend-setup)
- [Frontend Setup](#frontend-setup)
- [Running Locally](#running-locally)
- [Tech Stack](#tech-stack)
- [Troubleshooting](#troubleshooting)

---

## **Architecture**

### **Backend Stack**
- Framework: **ASP.NET Core 8.0**
- Database: **Microsoft SQL Server** (Model-First approach with EF Core)
- Authentication: **JWT Bearer tokens** + HTTP-only cookies
- API Docs: **Swagger/OpenAPI**
- Logging: **Serilog**

### **Frontend Stack**
- Framework: **React 19** with TypeScript
- Build Tool: **Vite**
- Styling: **Tailwind CSS 4** + shadcn/ui
- Forms: **React Hook Form** + **Zod**
- State: **React Context API** + AuthProvider

---

## **Backend Setup**

### **Prerequisites**
- .NET 8.0 SDK or higher
- SQL Server 2019+ (SQL Server Express)
- Visual Studio or VS Code

### **Environment Variables**

Your sensitive data is managed via **Windows environment variables**:

```
DB_PASS        → Your SQL Server password
JWT_SECRET     → Secret key for signing JWT tokens (32+ chars)
```

These are referenced in `appsettings.json` as `{DB_PASS}` and `{JWT_SECRET}` and substituted at runtime in `Program.cs`:

```csharp
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
connString = connString!.Replace("{DB_PASS}", Environment.GetEnvironmentVariable("DB_PASS") ?? "");
```

### **Installation**

```bash
cd ExpensesTrackerApp
dotnet restore
dotnet ef database update
dotnet run
```

API runs on `https://localhost:5001` | Swagger UI at `/swagger`

### **NuGet Packages**

| Package | Version | Purpose |
|---------|---------|---------|
| AutoMapper | 15.0.1 | DTO ↔ Model mapping |
| BCrypt.Net-Next | 4.0.3 | Password hashing |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.20 | JWT validation |
| Microsoft.EntityFrameworkCore | 9.0.9 | ORM |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.9 | SQL Server provider |
| Serilog.AspNetCore | 9.0.0 | Structured logging |
| Swashbuckle.AspNetCore | 6.6.2 | Swagger/OpenAPI |

---

## **Frontend Setup**

### **Prerequisites**
- Node.js 18+ and npm
- Modern web browser

### **Installation**

```bash
cd frontend
npm install
```

Create `.env.local` in frontend root:
```env
VITE_API_URL=https://localhost:5001
VITE_API_TIMEOUT=30000
```

```bash
npm run dev
```

App runs on `http://localhost:5173`

### **NPM Dependencies**

| Package | Version | Purpose |
|---------|---------|---------|
| react | 19.1.1 | Framework |
| react-router | 7.9.4 | Routing |
| react-hook-form | 7.65.0 | Form state |
| zod | 4.1.12 | Validation |
| tailwindcss | 4.1.16 | Styling |
| @radix-ui/react-* | Latest | UI components |
| lucide-react | 0.546.0 | Icons |
| jwt-decode | 4.0.0 | Token decoding |
| js-cookie | 3.0.5 | Cookie management |

---

## **Running Locally**

### **Terminal 1 - Backend**
```bash
cd ExpensesTrackerApp
dotnet run
# Runs on https://localhost:5001
```

### **Terminal 2 - Frontend**
```bash
cd frontend
npm run dev
# Runs on http://localhost:5173
```

---

## **Tech Stack**

### **Backend**
- C# / ASP.NET Core 8
- Entity Framework Core (Model-First)
- SQL Server
- JWT Authentication
- Serilog Logging

### **Frontend**
- React 19 + TypeScript
- Vite
- Tailwind CSS 4
- React Hook Form
- Zod Validation
- React Router

### **Database**
- **Model-First Approach:** C# models defined in code, `OnModelCreating()` configures relationships, migrations auto-generate SQL
- Migrations: `dotnet ef migrations add` → `dotnet ef database update`

---

## **API Endpoints**

### **Authentication**
- `POST /api/auth/login/access-token` — Login and receive JWT token

### **Protected Routes**
- Decorated with `[Authorize]` — Require `Authorization: Bearer <token>` header
- Role-based access with `[Authorize(Roles = "Admin")]`
- Returns `401 Unauthorized` if invalid, `403 Forbidden` if insufficient permissions

---

## **Authentication Flow**

1. User submits credentials to `/api/auth/login/access-token`
2. Backend verifies against database
3. JWT token generated with embedded claims (userId, username, email, role)
4. Token returned to frontend + stored in **HTTP-only cookie**
5. **AuthProvider** manages user context globally
6. Protected routes check auth status, redirect unauthenticated users to login
7. Token automatically sent with every request (CORS enabled)
8. Backend validates token signature, expiration, and issuer on each request

---

## **Troubleshooting**

| Issue | Solution |
|-------|----------|
| **Environment variables not found** | Restart VS/Terminal after setting Windows env vars |
| **JWT Token Invalid** | Verify `JWT_SECRET` env var is set correctly |
| **CORS Errors** | Check `AddCors()` in `Program.cs` allows `http://localhost:5173` |
| **DB Connection Failed** | Verify SQL Server is running; check `DB_PASS` env var |
| **Vite Port Taken** | Run `npm run dev -- --port 3000` |
| **Migrations Won't Apply** | Ensure SQL Server user has `db_owner` role |

---
