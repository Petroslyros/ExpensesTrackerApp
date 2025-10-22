using ExpensesTrackerApp.Data;

namespace ExpensesTrackerApp.Repositories.Interfaces
{
    public interface IBudgetRepository
    {
        Task<Budget?> GetBudgetByUserAndCategoryAsync(int userId, int categoryId);
        Task<List<Budget>> GetBudgetsByUserAsync(int userId);

        Task<Budget?> GetBudgetByIdAsync(int budgetId);
    }
}
