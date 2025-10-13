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


        public async Task<ExpenseReadOnlyDTO> CreateExpenseAsync(ExpenseInsertDTO expenseDto)
        {
            var user = await unitOfWork.UserRepository.GetAsync(expenseDto.UserId);
            if (user == null)
            {
                throw new EntityNotFoundException("ExpenseCategory", $"Category with Id {expenseDto.ExpenseCategoryId} not found");
            }
            var category = await unitOfWork.ExpenseCategoryRepository.GetAsync(expenseDto.ExpenseCategoryId);
            if (category == null)
            {
                throw new EntityNotFoundException("ExpenseCategory", $"Category with ID {expenseDto.ExpenseCategoryId} not found");
            }

            var expense = new Expense
            {
                Title = expenseDto.Title,
                Amount = expenseDto.Amount,
                Date = expenseDto.Date,
                UserId = expenseDto.UserId,
                ExpenseCategoryId = expenseDto.ExpenseCategoryId
            };
            await unitOfWork.ExpenseRepository.AddAsync(expense);
            await unitOfWork.SaveAsync();

            return new ExpenseReadOnlyDTO
                (
                    expense.Id,
                    expense.Title,
                    expense.Amount,
                    expense.Date,
                    new ExpenseCategoryReadOnlyDTO(category!.Id, category.Name)

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
