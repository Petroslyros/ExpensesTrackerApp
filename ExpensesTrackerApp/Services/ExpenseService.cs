using AutoMapper;
using ExpensesTrackerApp.Core.Filters;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Exceptions;
using ExpensesTrackerApp.Models;
using ExpensesTrackerApp.Repositories.Interfaces;
using ExpensesTrackerApp.Services.Interfaces;
using Serilog;

namespace ExpensesTrackerApp.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<ExpenseService> logger = new LoggerFactory().AddSerilog().CreateLogger<ExpenseService>();

        public ExpenseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }


        public async Task<ExpenseReadOnlyDTO> CreateExpenseAsync(ExpenseInsertDTO expenseDto, int userId)
        {
            // Validate user
            var user = await unitOfWork.UserRepository.GetAsync(userId);
            if (user == null)
                throw new EntityNotFoundException("User", $"User with Id {userId} not found");

            // Validate category name
            if (string.IsNullOrWhiteSpace(expenseDto.CategoryName))
                throw new Exception("Category name is required");

            // Check if category exists (case-insensitive)
            var category = await unitOfWork.ExpenseCategoryRepository
                .GetByNameAsync(expenseDto.CategoryName.Trim());

            if (category == null)
            {
                // Create new category
                category = new ExpenseCategory
                {
                    Name = expenseDto.CategoryName.Trim()
                };
                await unitOfWork.ExpenseCategoryRepository.AddAsync(category);
                await unitOfWork.SaveAsync(); // save new category to get Id
            }

            // Create expense linked to the category
            var expense = new Expense
            {
                Title = expenseDto.Title,
                Amount = expenseDto.Amount,
                Date = expenseDto.Date,
                UserId = userId,
                ExpenseCategoryId = category.Id
            };

            await unitOfWork.ExpenseRepository.AddAsync(expense);
            await unitOfWork.SaveAsync();

            // Return DTO
            return new ExpenseReadOnlyDTO(
                expense.Id,
                expense.Title,
                expense.Amount,
                expense.Date,
                new ExpenseCategoryReadOnlyDTO(category.Id, category.Name)
            );
        }

        public Task<bool> DeleteExpenseAsync(int expenseId)
        {
            throw new NotImplementedException();
        }

        public Task<ExpenseReadOnlyDTO?> GetByTitleAsync(string title)
        {
            throw new NotImplementedException();
        }

        public Task<ExpenseReadOnlyDTO?> GetExpenseByIdAsync(int expenseId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExpenseReadOnlyDTO>> GetExpensesByCategoryAsync(int userId, int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<ExpenseReadOnlyDTO>> GetPaginatedUserExpensesAsync(int userId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<ExpenseReadOnlyDTO>> GetPaginatedUserExpensesFilteredAsync(int userId, int pageNumber, int pageSize, ExpenseFiltersDTO filters)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalAmountByUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExpenseReadOnlyDTO>> GetUserExpensesAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateExpenseAsync(ExpenseInsertDTO expense)
        {
            throw new NotImplementedException();
        }
    }
}
