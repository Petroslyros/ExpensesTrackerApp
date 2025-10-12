using ExpensesTrackerApp.Core.Filters;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Models;

namespace ExpensesTrackerApp.Repositories.Interfaces
{
    public interface IUserService
    {
        //
        Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials);
        Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username);
        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize, UserFiltersDTO userFiltersDTO);
    }
}
