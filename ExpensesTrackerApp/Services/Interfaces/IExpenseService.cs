using ExpensesTrackerApp.Core.Filters;
using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Models;

namespace ExpensesTrackerApp.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseReadOnlyDTO?> GetExpenseByIdAsync(int expenseId);

        Task<PaginatedResult<ExpenseReadOnlyDTO>> GetPaginatedUserExpensesAsync(
            int userId, int pageNumber, int pageSize);

        Task<PaginatedResult<ExpenseReadOnlyDTO>> GetPaginatedUserExpensesFilteredAsync(
            int userId, int pageNumber, int pageSize, ExpenseFiltersDTO filters);

        Task<ExpenseReadOnlyDTO> CreateExpenseAsync(ExpenseInsertDTO newExpense, int userId);

        Task<decimal> GetTotalAmountByUserAsync(int userId);

        Task<List<ExpenseReadOnlyDTO>> GetUserExpensesAsync(int userId);

        Task<List<ExpenseReadOnlyDTO>> GetExpensesByCategoryAsync(int userId, int categoryId);

        Task<ExpenseReadOnlyDTO?> GetByTitleAsync(string title);

        Task<bool> UpdateExpenseAsync(ExpenseInsertDTO expense);

        Task<bool> DeleteExpenseAsync(int expenseId);
    }
}
