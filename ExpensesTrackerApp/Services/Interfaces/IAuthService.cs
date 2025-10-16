using ExpensesTrackerApp.Core.Enums;

namespace ExpensesTrackerApp.Services.Interfaces
{
    public interface IAuthService
    {
        string CreateUserToken(int userId, string username, string email, UserRole userRole, string appSecurityKey);
    }
}
