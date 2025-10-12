using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Models;
using System.Linq.Expressions;

namespace ExpensesTrackerApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(string username, string password);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
           List<Expression<Func<User, bool>>> predicates);
    }
}
