using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Repositories.Interfaces;

namespace ExpensesTrackerApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExpenseAppDbContext _context;

        public UnitOfWork(ExpenseAppDbContext context)
        {
            _context = context;
        }




        //implementation of getter with expression-bodied property
        public IUserRepository UserRepository => new UserRepository(_context);

        public IExpenseRepository ExpenseRepository => new ExpenseRepository(_context);

        public IExpenseCategoryRepository ExpenseCategoryRepository => new ExpenseCategoryRepository(_context);

        public IBudgetRepository BudgetRepository => new BudgetRepository(_context);

        //for commit and rolback
        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
