using ExpensesTrackerApp.Data;

namespace ExpensesTrackerApp.Repositories.Interfaces
{
    public interface IExpenseCategoryRepository : IBaseRepository<ExpenseCategory>
    {
        Task<ExpenseCategory?> GetByNameAsync(string name);
    }
}
