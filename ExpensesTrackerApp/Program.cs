
using ExpensesTrackerApp.Configuration;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Repositories;
using ExpensesTrackerApp.Services;
using ExpensesTrackerApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ExpensesTrackerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");
            connString = connString!.Replace("{DB_PASS}", Environment.GetEnvironmentVariable("DB_PASS") ?? "");

            builder.Services.AddDbContext<ExpenseAppDbContext>(options => options.UseSqlServer(connString));  // DBContext with our DB

            builder.Services.AddRepositories();                                     // User of repos
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());  //Mapper
            builder.Host.UseSerilog((ctx, lc) =>                                    //Serilog
                lc.ReadFrom.Configuration(ctx.Configuration));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
