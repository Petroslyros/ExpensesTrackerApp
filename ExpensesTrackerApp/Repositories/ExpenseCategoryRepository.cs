using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesTrackerApp.Repositories
{
    public class ExpenseCategoryRepository : BaseRepository<ExpenseCategory>, IExpenseCategoryRepository
    {
        public ExpenseCategoryRepository(ExpenseAppDbContext context) : base(context) { }


        public async Task<ExpenseCategory?> GetByNameAsync(string name)
        {
            return await dbSet
            .Where(c => c.Name == name)  // filter by category name
            .SingleOrDefaultAsync();     // return one or null
        }


    }
}
