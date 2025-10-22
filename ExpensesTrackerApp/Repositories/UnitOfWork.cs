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
        public UserRepository UserRepository => new(_context);

        public ExpenseRepository ExpenseRepository => new(_context);

        public ExpenseCategoryRepository ExpenseCategoryRepository => new(_context);

        public BudgetRepository BudgetRepository => new(_context);

        //for commit and rolback
        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
