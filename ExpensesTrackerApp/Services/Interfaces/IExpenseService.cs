using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Models;

namespace ExpensesTrackerApp.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseReadOnlyDTO?> GetExpenseByIdAsync(int expenseId);

        Task<PaginatedResult<ExpenseReadOnlyDTO>> GetPaginatedUserExpensesAsync(
            int userId, int pageNumber, int pageSize);

        Task<ExpenseReadOnlyDTO> CreateExpenseAsync(ExpenseInsertDTO newExpense, int userId);

        Task<decimal> GetTotalAmountByUserAsync(int userId);

        Task<List<ExpenseReadOnlyDTO>> GetExpensesByCategoryAsync(int userId, int categoryId);

        Task<ExpenseReadOnlyDTO?> GetByTitleAsync(string title);

        Task<ExpenseReadOnlyDTO> UpdateExpenseAsync(int userId, int expenseId, ExpenseInsertDTO expenseDto);

        Task DeleteExpenseAsync(int expenseId, int userId);
    }
}
