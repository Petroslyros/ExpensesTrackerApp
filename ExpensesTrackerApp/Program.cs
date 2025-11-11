using ExpensesTrackerApp.Configuration;
using ExpensesTrackerApp.Core.Helpers;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Repositories;
using ExpensesTrackerApp.Services;
using ExpensesTrackerApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System.Text;

namespace ExpensesTrackerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Get all environment variables
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPass = Environment.GetEnvironmentVariable("DB_PASS");
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

            // Validate required environment variables
            if (string.IsNullOrEmpty(dbPass))
            {
                throw new InvalidOperationException(
                    "DB_PASS environment variable is not set! " +
                    "Please set it using: setx DB_PASS \"your_password\""
                );
            }

            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException(
                    "JWT_SECRET environment variable is not set! " +
                    "Please set it using: setx JWT_SECRET \"your_secret_key\""
                );
            }

            // Build connection string from environment variables
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");
            connString = connString!
                .Replace("{DB_HOST}", dbHost)
                .Replace("{DB_PORT}", dbPort)
                .Replace("{DB_NAME}", dbName)
                .Replace("{DB_USER}", dbUser)
                .Replace("{DB_PASS}", dbPass);

            builder.Services.AddDbContext<ExpenseAppDbContext>(options => options.UseSqlServer(connString));

            // Add UnitOfWork DI to the scope of IoC container
            builder.Services.AddRepositories();

            // Add AplicationService DI to the scope of IoC container
            builder.Services.AddScoped<IApplicationService, ApplicationService>();

            // AutoMapper setup
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());

            // Sets up Serilog for structured logging
            builder.Host.UseSerilog((ctx, lc) =>
                lc.ReadFrom.Configuration(ctx.Configuration));

            // This configures JWT-based authentication using bearer tokens
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://localhost:5001",

                    ValidateAudience = true,
                    ValidAudience = "https://localhost:5001",

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                };
            });

            // Enables Cross-Origin Resource Sharing
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            // Registers controller support for MVC-style routing
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            // Provides info and use of APIs
            builder.Services.AddEndpointsApiExplorer();

            // Configures Swagger UI and security schemes
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Expense Tracker App", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token.\r\nExample: \"Bearer eyJhbGciOiJI...\""
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Expenses App v1"));
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}