Project Overview
A full-stack web application for tracking personal expenses. The backend is built with ASP.NET Core 8 and the frontend with React 19, communicating via REST API with JWT-based authentication.

Architecture
Backend Stack

Framework: ASP.NET Core 8.0
Database: Microsoft SQL Server (Model-First approach with EF Core)
Authentication: JWT Bearer tokens with HTTP-only cookies
API Documentation: Swagger/OpenAPI
Logging: Serilog

Frontend Stack

Framework: React 19 with TypeScript
Build Tool: Vite
Styling: Tailwind CSS 4 with shadcn/ui components
Form Management: React Hook Form + Zod validation
HTTP Client: Fetch API with custom interceptors
State Management: React Context API + AuthProvider


Backend Setup & Deployment
Prerequisites

.NET 8.0 SDK or higher
SQL Server 2019+ (SQL Server Express or higher)
Visual Studio or Visual Studio Code
Windows environment (for environment variable configuration)
