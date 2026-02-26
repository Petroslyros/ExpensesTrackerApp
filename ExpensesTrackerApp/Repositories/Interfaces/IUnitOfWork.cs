namespace ExpensesTrackerApp.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; } // Πρόσθεσε το I
        IExpenseRepository ExpenseRepository { get; }
        IExpenseCategoryRepository ExpenseCategoryRepository { get; }
        IBudgetRepository BudgetRepository { get; }
        Task<bool> SaveAsync();
    }
}
