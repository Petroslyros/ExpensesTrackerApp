
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

            // Register Entity Framework Core with SQL Server context with DI using config connection string
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");
            connString = connString!.Replace("{DB_PASS}", Environment.GetEnvironmentVariable("DB_PASS") ?? "");

            builder.Services.AddDbContext<ExpenseAppDbContext>(options => options.UseSqlServer(connString));  // DBContext with our DB

            // Add UnitOfWork DI to the scope of IoC container
            builder.Services.AddRepositories();

            // Add AplicationService DI to the scope of IoC container
            builder.Services.AddScoped<IApplicationService, ApplicationService>();

            // AutoMapper setup. Automatically registers all AutoMapper profiles found in the same assembly as MapperConfig
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());

            // Sets up Serilog for structured logging. Reads config from appsettings.json
            builder.Host.UseSerilog((ctx, lc) =>
                lc.ReadFrom.Configuration(ctx.Configuration));


            // This configures JWT-based authentication using bearer tokens
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // in case Kestrel gives 401 to prove identity on the initial requiest we do the challenge
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtSettinsg = builder.Configuration.GetSection("Authentication");
                options.IncludeErrorDetails = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://localhost:5001",

                    ValidateAudience = true,
                    ValidAudience = "https://localhost:5001",

                    ValidateLifetime = true, //ensure not expired
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettinsg["SecretKey"]!))

                };
            });

            // Enables Cross-Origin Resource Sharing so front-end clients can communicate with the API.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",  // unrestricted access (good for testing).
                    b => b.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            //builder.Services.AddCors(options => {
            //    options.AddPolicy("AllowReactClient",  // restricted to React dev server at port 5173.
            //        b => b.WithOrigins("http://localhost:5173/") // Assuming React runs on localhost:5173
            //              .AllowAnyMethod()
            //              .AllowAnyHeader());
            //});
            //builder.Services.AddCors(options => {
            //    options.AddPolicy("AllowDockerClient",  // restricted to Docker composed frontend at port 3000.
            //        b => b.WithOrigins("http://localhost:3000/") // Assuming Docker frontend runs on localhost:3000
            //              .AllowAnyMethod()
            //              .AllowAnyHeader());
            //});



            // Registers controller support for MVC - style routing.
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });


            // Provides info and use of APIs 
            builder.Services.AddEndpointsApiExplorer();

            // Configures Swagger UI and security schemes(JWT auth support)
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Expense Tracker App", Version = "v1" });

                // Define the BearerAuth scheme
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token.\r\nExample: \"Bearer eyJhbGciOiJI...\""
                });

                // Require token globally for endpoints marked with [Authorize]
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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Expenses App v1"));
            }



            app.UseHttpsRedirection();

            //Applies CORS rules 
            app.UseCors("AllowAll");

            //Errorhandler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // Applies Authentication middleware
            app.UseAuthentication();

            app.UseAuthorization();



            //Maps controller endpoints to route requests.
            app.MapControllers();

            app.Run();
        }
    }
}
