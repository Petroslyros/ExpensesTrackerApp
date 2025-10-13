using ExpensesTrackerApp.Data;

namespace ExpensesTrackerApp.Repositories.Interfaces
{
    public interface IExpenseCategoryRepository
    {
        Task<ExpenseCategory?> GetByNameAsync(string name);
    }
}
