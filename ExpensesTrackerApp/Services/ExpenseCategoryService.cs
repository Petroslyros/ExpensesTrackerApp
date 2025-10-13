using AutoMapper;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Models;
using ExpensesTrackerApp.Repositories.Interfaces;
using Serilog;
using System.Linq.Expressions;

namespace ExpensesTrackerApp.Services
{
    public class ExpenseCategoryService : IExpenseRepository
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<ExpenseCategoryService> logger = new LoggerFactory().AddSerilog().CreateLogger<ExpenseCategoryService>();

        public ExpenseCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public Task<Expense> GetByTitleAsync(string title)
        {
            throw new NotImplementedException();
        }

        public Task<Expense?> GetExpenseByIdAsync(int expenseId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Expense>> GetExpensesByCategoryAsync(int userId, int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<Expense>> GetPaginatedUserExpensesAsync(int userId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<Expense>> GetPaginatedUserExpensesFilteredAsync(int userId, int pageNumber, int pageSize, List<Expression<Func<Expense, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalAmountByUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Expense>> GetUserExpensesAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
