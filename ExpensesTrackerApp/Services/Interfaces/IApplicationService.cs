namespace ExpensesTrackerApp.Services.Interfaces
{
    public interface IApplicationService
    {
        UserService UserService { get; }
        ExpenseService ExpenseService { get; }
        ExpenseCategoryService ExpenseCategoryService { get; }
    }
}
