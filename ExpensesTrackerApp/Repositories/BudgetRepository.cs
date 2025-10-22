using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesTrackerApp.Repositories
{
    public class BudgetRepository : BaseRepository<Budget>, IBudgetRepository
    {
        public BudgetRepository(ExpenseAppDbContext context) : base(context) { }

        public async Task<Budget?> GetBudgetByIdAsync(int budgetId)
        {
            return await dbSet
              .Include(b => b.User)
              .Include(b => b.Category)
              .FirstOrDefaultAsync(b => b.Id == budgetId);
        }

        public async Task<Budget?> GetBudgetByUserAndCategoryAsync(int userId, int categoryId)
        {
            return await dbSet
                .Include(b => b.Category)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId);
        }

        public async Task<List<Budget>> GetBudgetsByUserAsync(int userId)
        {
            return await dbSet
                .Where(b => b.UserId == userId)
                .Include(b => b.User)
                .Include(b => b.Category)
                .ToListAsync();
        }
    }
}
