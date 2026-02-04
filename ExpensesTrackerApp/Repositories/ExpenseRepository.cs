using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesTrackerApp.Repositories
{
    public class ExpenseRepository : BaseRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ExpenseAppDbContext context) : base(context) { }


        public async Task<Expense?> GetByTitleAsync(string? title) // nullability here forces the caller to handle it properly
        {
            return await context.Expenses
                .Where(e => e.Title == title)
                .FirstOrDefaultAsync(); //fetched zero or one , or can return null
        }

        public async Task<Expense?> GetExpenseByIdAsync(int expenseId)
        {
            return await dbSet
                .FirstOrDefaultAsync(e => e.Id == expenseId);
            //include (eager load) user and expense category for the ReadOnlyDTO
        }

        public async Task<List<Expense>> GetExpensesByCategoryAsync(int userId, int categoryId)
        {
            return await dbSet
                .Where(e => e.UserId == userId && e.ExpenseCategoryId == categoryId)
                .Include(e => e.ExpenseCategory) //eager load
                .Include(e => e.User)             //eager load 
                .ToListAsync();
        }

        /// <summary>
        /// Returns paginated expenses for a specific user, including category navigation.
        /// </summary>
        public async Task<List<Expense>> GetPaginatedExpensesByUserAsync(int userId, int pageNumber, int pageSize)
        {
            return await dbSet
                .Where(e => e.UserId == userId)
                .Include(e => e.ExpenseCategory) // eager load category
                .OrderByDescending(e => e.Date) // optional: latest first
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Returns total number of expenses for a specific user.
        /// </summary>
        public async Task<int> GetCountByUserAsync(int userId)
        {
            return await dbSet
                .Where(e => e.UserId == userId)
                .CountAsync();
        }


        public async Task<decimal> GetTotalAmountByUserAsync(int userId)
        {
            return await dbSet
                .Where(e => e.UserId == userId)
                .SumAsync(e => e.Amount);
        }


        public async Task<List<Expense>> GetUserExpensesAsync(int userId)
        {
            return await dbSet
            .Where(e => e.UserId == userId)       // filter by user
            .Include(e => e.ExpenseCategory)      // eager load category if needed
            .Include(e => e.User)                 // eager load user if needed
            .ToListAsync();                        // execute query
        }

        public async Task<bool> IsCategoryUsedAsync(int categoryId)
        {
            return await context.Expenses.AnyAsync(e => e.ExpenseCategoryId == categoryId);
        }

        //search expense by title
        public async Task<List<Expense>> SearchByTitleAsync(int userId, string searchTerm)
        {
            return await dbSet
                .Where(e => e.UserId == userId && e.Title.Contains(searchTerm))
                .Include(e => e.ExpenseCategory)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }


    }
}
