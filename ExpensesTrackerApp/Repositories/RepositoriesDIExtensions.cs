using ExpensesTrackerApp.Repositories.Interfaces;

namespace ExpensesTrackerApp.Repositories
{
    // extention method to be added in the IOC container so it can use all the repositories together
    public static class RepositoriesDIExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
