using ExpensesTrackerApp.Data;

namespace ExpensesTrackerApp.Repositories.Interfaces
{
    public interface IExpenseRepository : IBaseRepository<Expense>
    {
        Task<List<Expense>> GetUserExpensesAsync(int userId);
        Task<Expense?> GetExpenseByIdAsync(int expenseId);
        Task<decimal> GetTotalAmountByUserAsync(int userId);
        Task<Expense?> GetByTitleAsync(string title);
        Task<List<Expense>> GetExpensesByCategoryAsync(int userId, int categoryId);
        Task<List<Expense>> GetPaginatedExpensesByUserAsync(int userId, int pageNumber, int pageSize);
        Task<int> GetCountByUserAsync(int userId);
        Task<bool> IsCategoryUsedAsync(int categoryId);
        Task<List<Expense>> SearchByTitleAsync(int userId, string searchTerm);

    }
}
