# **Expense Tracker Application** 

A full-stack web application for tracking personal expenses. The backend is built with **ASP.NET Core 8** and the frontend with **React 19**, communicating via REST API with **JWT-based authentication**.

**📌 Note:** Backend and Frontend are in separate repositories.

---

## **Table of Contents**
- [Architecture](#architecture)
- [Repository Links](#repository-links)
- [Backend Setup](#backend-setup)
- [Frontend Setup](#frontend-setup)
- [Running Locally](#running-locally)
- [Tech Stack](#tech-stack)
- [Troubleshooting](#troubleshooting)

---
## Demo Video
**Tip:** to open the demo in a new tab, middle-click the thumbnail (or Cmd/Ctrl + click, or right-click → “Open link in new tab”).

[![Watch the demo on YouTube](https://img.youtube.com/vi/0l5iLTNP8Lo/maxresdefault.jpg)](https://www.youtube.com/watch?v=0l5iLTNP8Lo)

**Features Demonstrated:**
- User authentication with JWT
- Expense logging and categorization
- Real-time budget tracking with visual indicators
- Responsive mobile-friendly design
- Expense management (create, edit, delete)


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

## **Repository Links**

- **Backend Repository:** [expense-tracker-api](https://github.com/YOUR_USERNAME/expense-tracker-api)
- **Frontend Repository:** [expense-tracker-web](https://github.com/YOUR_USERNAME/expense-tracker-web)

---

## **Backend Setup**

### **Prerequisites**
- .NET 8.0 SDK or higher
- SQL Server 2019+ (SQL Server Express) - **Download from:** https://www.microsoft.com/en-us/sql-server/sql-server-express
- SQL Server Management Studio (SSMS) - optional but recommended for managing databases
- Visual Studio or VS Code
- Entity Framework Core CLI (install with: `dotnet tool install --global dotnet-ef`)

### **Clone & Install**

```bash
git clone https://github.com/YOUR_USERNAME/expense-tracker-api.git
cd expense-tracker-api
```

### **Environment Variables**

All sensitive data is managed via **Windows environment variables**. Set these before running the backend:

```
DB_HOST        → Database host (e.g., localhost)
DB_PORT        → Database port (e.g., SQLEXPRESS)
DB_NAME        → Database name (e.g., ExpensesDbApi)
DB_USER        → Database user (e.g., Petros)
DB_PASS        → Database password
JWT_SECRET     → Secret key for signing JWT tokens (32+ chars)
```

**Set them using Command Prompt as Administrator:**

```bash
setx DB_HOST "localhost"
setx DB_PORT "SQLEXPRESS"
setx DB_NAME "ExpensesDbApi"
setx DB_USER "Petros"
setx DB_PASS "your_sql_server_password"
setx JWT_SECRET "your_jwt_secret_key"
```

**Restart Visual Studio** after setting these variables.

### **Installation**

1. **Install Entity Framework Core CLI (one-time setup):**
   ```bash
   dotnet tool install --global dotnet-ef --version 9.0.10
   ```

2. **Restore packages and create database:**
   ```bash
   dotnet restore
   dotnet ef database update
   dotnet run
   ```

If `dotnet ef database update` fails, manually create the database in SQL Server Management Studio:
- Open **SQL Server Management Studio**
- Right-click **Databases** → **New Database**
- Name it `ExpensesDbApi` (or your `DB_NAME` env var)
- Click OK
- Then run `dotnet ef database update` again

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

### **Clone & Install**

```bash
git clone https://github.com/YOUR_USERNAME/expense-tracker-web.git
cd expense-tracker-web
npm install
```

### **Environment Configuration**

Create `.env.local` in the project root:

```env
VITE_API_URL=https://localhost:5001
VITE_API_TIMEOUT=30000
```

### **Running the App**

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
cd expense-tracker-api

# One-time setup (if not done before)
dotnet tool install --global dotnet-ef --version 9.0.10

dotnet restore
dotnet ef database update
dotnet run
# Runs on https://localhost:5001
```

### **Terminal 2 - Frontend**

```bash
cd expense-tracker-web
npm install
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
| **SQL Server not installed** | Download SQL Server Express from https://www.microsoft.com/en-us/sql-server/sql-server-express and install it |
| **SQL Server not running** | Start SQL Server from Windows Services (search "Services" in Windows) or SQL Server Configuration Manager |
| **Environment variables not found** | Run setx commands as Administrator, then restart Visual Studio |
| **DB Connection Failed** | Verify SQL Server is running; check all DB environment variables (DB_HOST, DB_PORT, DB_NAME, DB_USER, DB_PASS) |
| **Database doesn't exist** | Manually create the database in SQL Server Management Studio with the name from DB_NAME env var, then run dotnet ef database update |
| **JWT Token Invalid** | Verify JWT_SECRET env var is set correctly and is 32+ characters |
| **CORS Errors** | Check AddCors() in Program.cs allows http://localhost:5173 |
| **Vite Port Taken** | Run npm run dev -- --port 3000 |
| **Migrations Won't Apply** | Ensure SQL Server user has db_owner role; verify database exists in SQL Server |
| **"Cannot connect to database"** | Verify SQL Server instance name matches DB_PORT env var (usually SQLEXPRESS for local installs) |

