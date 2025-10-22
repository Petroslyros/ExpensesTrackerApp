namespace ExpensesTrackerApp.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        UserRepository UserRepository { get; }
        ExpenseRepository ExpenseRepository { get; }
        ExpenseCategoryRepository ExpenseCategoryRepository { get; }

        BudgetRepository BudgetRepository { get; }

        Task<bool> SaveAsync();
    }
}
