Expense Tracker Application 
A full-stack web application for tracking personal expenses. The backend is built with ASP.NET Core 8 and the frontend with React 19, communicating via REST API with JWT-based authentication.
📌 Note: Backend and Frontend are in separate repositories.

Table of Contents

Architecture
Repository Links
Backend Setup
Frontend Setup
Running Locally
Tech Stack
Troubleshooting


Architecture
Backend Stack

Framework: ASP.NET Core 8.0
Database: Microsoft SQL Server (Model-First approach with EF Core)
Authentication: JWT Bearer tokens + HTTP-only cookies
API Docs: Swagger/OpenAPI
Logging: Serilog

Frontend Stack

Framework: React 19 with TypeScript
Build Tool: Vite
Styling: Tailwind CSS 4 + shadcn/ui
Forms: React Hook Form + Zod
State: React Context API + AuthProvider


Repository Links

Backend Repository: expense-tracker-api
Frontend Repository: expense-tracker-web


Backend Setup
Prerequisites

.NET 8.0 SDK or higher
SQL Server 2019+ (SQL Server Express) - Download from: https://www.microsoft.com/en-us/sql-server/sql-server-express
SQL Server Management Studio (SSMS) - optional but recommended for managing databases
Visual Studio or VS Code
Entity Framework Core CLI (install with: dotnet tool install --global dotnet-ef --version 9.0.10)

Clone & Install
bashgit clone https://github.com/YOUR_USERNAME/expense-tracker-api.git
cd expense-tracker-api
Environment Variables
All sensitive data is managed via Windows environment variables. Set these before running the backend:
DB_HOST        → Database host (e.g., localhost)
DB_PORT        → Database port (e.g., SQLEXPRESS)
DB_NAME        → Database name (e.g., ExpensesDbApi)
DB_USER        → Database user (e.g., Petros)
DB_PASS        → Database password
JWT_SECRET     → Secret key for signing JWT tokens (32+ chars)
Set them using Command Prompt as Administrator:
bashsetx DB_HOST "localhost"
setx DB_PORT "SQLEXPRESS"
setx DB_NAME "ExpensesDbApi"
setx DB_USER "Petros"
setx DB_PASS "your_sql_server_password"
setx JWT_SECRET "your_jwt_secret_key"
Restart Visual Studio after setting these variables.
Installation

Install Entity Framework Core CLI (one-time setup):

bash   dotnet tool install --global dotnet-ef 

Restore packages and create database:

bash   dotnet restore
   dotnet ef database update
   dotnet run
If dotnet ef database update fails, manually create the database in SQL Server Management Studio:

Open SQL Server Management Studio
Right-click Databases → New Database
Name it ExpensesDbApi (or your DB_NAME env var)
Click OK
Then run dotnet ef database update again

API runs on https://localhost:5001 | Swagger UI at /swagger
NuGet Packages
PackageVersionPurposeAutoMapper15.0.1DTO ↔ Model mappingBCrypt.Net-Next4.0.3Password hashingMicrosoft.AspNetCore.Authentication.JwtBearer8.0.20JWT validationMicrosoft.EntityFrameworkCore9.0.9ORMMicrosoft.EntityFrameworkCore.SqlServer9.0.9SQL Server providerSerilog.AspNetCore9.0.0Structured loggingSwashbuckle.AspNetCore6.6.2Swagger/OpenAPI

Frontend Setup
Prerequisites

Node.js 18+ and npm
Modern web browser

Clone & Install
bashgit clone https://github.com/YOUR_USERNAME/expense-tracker-web.git
cd expense-tracker-web
npm install
Environment Configuration
Create .env.local in the project root:
envVITE_API_URL=https://localhost:5001
VITE_API_TIMEOUT=30000
Running the App
bashnpm run dev
App runs on http://localhost:5173
NPM Dependencies
PackageVersionPurposereact19.1.1Frameworkreact-router7.9.4Routingreact-hook-form7.65.0Form statezod4.1.12Validationtailwindcss4.1.16Styling@radix-ui/react-*LatestUI componentslucide-react0.546.0Iconsjwt-decode4.0.0Token decodingjs-cookie3.0.5Cookie management

Running Locally
Terminal 1 - Backend
bashcd expense-tracker-api

# One-time setup (if not done before)
dotnet tool install --global dotnet-ef --version 9.0.10

dotnet restore
dotnet ef database update
dotnet run
# Runs on https://localhost:5001
Terminal 2 - Frontend
bashcd expense-tracker-web
npm install
npm run dev
# Runs on http://localhost:5173

Tech Stack
Backend

C# / ASP.NET Core 8
Entity Framework Core (Model-First)
SQL Server
JWT Authentication
Serilog Logging

Frontend

React 19 + TypeScript
Vite
Tailwind CSS 4
React Hook Form
Zod Validation
React Router

Database

Model-First Approach: C# models defined in code, OnModelCreating() configures relationships, migrations auto-generate SQL
Migrations: dotnet ef migrations add → dotnet ef database update


API Endpoints
Authentication

POST /api/auth/login/access-token — Login and receive JWT token

Protected Routes

Decorated with [Authorize] — Require Authorization: Bearer <token> header
Role-based access with [Authorize(Roles = "Admin")]
Returns 401 Unauthorized if invalid, 403 Forbidden if insufficient permissions


Authentication Flow

User submits credentials to /api/auth/login/access-token
Backend verifies against database
JWT token generated with embedded claims (userId, username, email, role)
Token returned to frontend + stored in HTTP-only cookie
AuthProvider manages user context globally
Protected routes check auth status, redirect unauthenticated users to login
Token automatically sent with every request (CORS enabled)
Backend validates token signature, expiration, and issuer on each request


Troubleshooting
IssueSolutionSQL Server not installedDownload SQL Server Express from https://www.microsoft.com/en-us/sql-server/sql-server-express and install itSQL Server not runningStart SQL Server from Windows Services (search "Services" in Windows) or SQL Server Configuration ManagerEnvironment variables not foundRun setx commands as Administrator, then restart Visual StudioDB Connection FailedVerify SQL Server is running; check all DB environment variables (DB_HOST, DB_PORT, DB_NAME, DB_USER, DB_PASS)Database doesn't existManually create the database in SQL Server Management Studio with the name from DB_NAME env var, then run dotnet ef database updateJWT Token InvalidVerify JWT_SECRET env var is set correctly and is 32+ charactersCORS ErrorsCheck AddCors() in Program.cs allows http://localhost:5173Vite Port TakenRun npm run dev -- --port 3000Migrations Won't ApplyEnsure SQL Server user has db_owner role; verify database exists in SQL Server"Cannot connect to database"Verify SQL Server instance name matches DB_PORT env var (usually SQLEXPRESS for local installs)
