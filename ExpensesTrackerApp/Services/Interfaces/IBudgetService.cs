using ExpensesTrackerApp.DTO;

namespace ExpensesTrackerApp.Services.Interfaces
{
    public interface IBudgetService
    {
        Task<BudgetReadOnlyDTO> CreateBudgetAsync(int userId, BudgetInsertDTO dto);
        Task<IEnumerable<BudgetReadOnlyDTO>> GetBudgetsByUserAsync(int userId);
        Task DeleteBudgetAsync(int budgetId);
    }
}
