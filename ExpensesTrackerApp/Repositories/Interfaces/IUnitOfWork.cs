namespace ExpensesTrackerApp.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        UserRepository UserRepository { get; }
        ExpenseRepository ExpenseRepository { get; }
        ExpenseCategoryRepository ExpenseCategoryRepository { get; }

        Task<bool> SaveAsync();
    }
}
