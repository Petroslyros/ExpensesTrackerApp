using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Models;
using System.Linq.Expressions;

namespace ExpensesTrackerApp.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task<List<Expense>> GetUserExpensesAsync(int userId);
        Task<Expense?> GetExpenseByIdAsync(int expenseId);
        Task<decimal> GetTotalAmountByUserAsync(int userId);
        Task<Expense> GetByTitleAsync(string title);
        Task<List<Expense>> GetExpensesByCategoryAsync(int userId, int categoryId);
        Task<PaginatedResult<Expense>> GetPaginatedUserExpensesAsync(int userId, int pageNumber, int pageSize);

        Task<PaginatedResult<Expense>> GetPaginatedUserExpensesFilteredAsync(
            int userId,
            int pageNumber,
            int pageSize,
            List<Expression<Func<Expense, bool>>> predicates);


    }
}
