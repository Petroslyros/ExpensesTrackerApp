using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Models;
using ExpensesTrackerApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExpensesTrackerApp.Repositories
{
    public class ExpenseRepository : BaseRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ExpenseAppDbContext context) : base(context) { }


        public async Task<Expense?> GetByTitleAsync(string? title)
        {
            return await context.Expenses
                .Where(e => e.Title == title)
                .FirstOrDefaultAsync(); //fetched zero or one
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

        public async Task<PaginatedResult<Expense>> GetPaginatedUserExpensesAsync(int userId, int pageNumber, int pageSize)
        {
            // Filter only the user’s expenses
            var query = dbSet
                .Where(e => e.UserId == userId)
                .Include(e => e.ExpenseCategory) // eager load category if needed
                .Include(e => e.User);           // eager load user if needed

            // Total count for pagination
            var totalRecords = await query.CountAsync();

            // Get the requested page
            var expenses = await query
                .OrderByDescending(e => e.Date) // optional: newest first
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return a paginated result
            return new PaginatedResult<Expense>
            {
                Data = expenses,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResult<Expense>> GetPaginatedUserExpensesFilteredAsync(int userId, int pageNumber, int pageSize, List<Expression<Func<Expense, bool>>> predicates)
        {

            //queryable object for this users expenses
            IQueryable<Expense> query = context.Expenses
                .Where(e => e.UserId == userId)
                .Include(e => e.ExpenseCategory)  // eager load the category
                .Include(e => e.User);            // eager load the user;


            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }
            //get count before pagination
            int totalRecords = await query.CountAsync();

            //paginate after filters
            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Expense>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

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
    }
}
