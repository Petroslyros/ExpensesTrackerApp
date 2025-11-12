using AutoMapper;
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

        /// <summary>
        /// Creates a new expense for the given user, automatically creating the category if it doesn't exist.
        /// </summary>
        /// <param name="expenseDto">Expense details including title, amount, date, and category name.</param>
        /// <param name="userId">The ID of the user creating the expense.</param>
        /// <returns>A DTO containing the created expense and its category.</returns>
        /// <exception cref="EntityNotFoundException">Thrown if the user does not exist.</exception>
        /// <exception cref="Exception">Thrown if the category name is missing or invalid.</exception>
        public async Task<ExpenseReadOnlyDTO> CreateExpenseAsync(ExpenseInsertDTO expenseDto, int userId)
        {
            // Validate user
            var user = await unitOfWork.UserRepository.GetAsync(userId);
            if (user == null)
                throw new EntityNotFoundException("User", $"User with Id {userId} not found");

            // Validate category name
            if (string.IsNullOrWhiteSpace(expenseDto.CategoryName))
                throw new InvalidArgumentException("Category", "Category name is required");

            // Check if Category Entity exists inside the db
            var category = await unitOfWork.ExpenseCategoryRepository
                .GetByNameAsync(expenseDto.CategoryName.Trim());

            if (category == null)
            {
                // Create new category with .Trim() so we wont have duplicates with whitespaces inside the DB
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

            logger.LogInformation("Expense created successfully for user {UserId} in category {CategoryId}", userId, category.Id);


            // Return DTO
            return mapper.Map<ExpenseReadOnlyDTO>(expense);
        }

        /// <summary>
        /// Deletes an expense belonging to the specified user. 
        /// Also removes the category if it is no longer used by any expenses.
        /// </summary>
        /// <param name="expenseId">The ID of the expense to delete.</param>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <exception cref="EntityNotFoundException">Thrown if the expense does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if the expense belongs to another user.</exception>
        /// <exception cref="Exception">Thrown if the deletion fails.</exception>
        public async Task DeleteExpenseAsync(int expenseId, int userId)
        {
            //  Get the expense
            var expense = await unitOfWork.ExpenseRepository.GetAsync(expenseId);
            if (expense == null)
            {
                logger.LogWarning("Expense with Id {ExpenseId} not found", expenseId);
                throw new EntityNotFoundException("Expense", $"Expense with Id {expenseId} not found");
            }

            if (expense.UserId != userId)
            {
                logger.LogWarning("The expense {ExpenseId} that doesn't belong to the user with {UserId}", expenseId, userId);
                throw new EntityNotAuthorizedException("Expense", "You cannot delete another user's expense");
            }

            int? categoryId = expense.ExpenseCategoryId;

            bool deleted = await unitOfWork.ExpenseRepository.DeleteAsync(expenseId);
            if (!deleted)
                throw new ServerException("ExpenseDeleteFailed", $"Failed to delete expense with Id {expenseId}");


            await unitOfWork.SaveAsync();
            logger.LogInformation("Expense {ExpenseId} deleted successfully by user {UserId}", expenseId, userId);

            // Check if the category is still used
            if (categoryId != null)
            {
                var isCategoryUsed = await unitOfWork.ExpenseRepository.IsCategoryUsedAsync(categoryId.Value);
                if (!isCategoryUsed)
                {
                    bool categoryDeleted = await unitOfWork.ExpenseCategoryRepository.DeleteAsync(categoryId.Value);
                    if (categoryDeleted)
                    {
                        await unitOfWork.SaveAsync();
                        logger.LogInformation("Category {CategoryId} deleted because it was unused", categoryId);
                    }
                }
            }


        }

        public Task<ExpenseReadOnlyDTO?> GetByTitleAsync(string title)
        {
            throw new NotImplementedException();
        }

        public async Task<ExpenseReadOnlyDTO?> GetExpenseByIdAsync(int expenseId)
        {
            Expense? expense = null;
            try
            {
                expense = await unitOfWork.ExpenseRepository.GetAsync(expenseId);
                logger.LogInformation("Expense found with ID : {Id}", expenseId);
            }
            catch (Exception ex)
            {
                logger.LogError("Error retreiving expese with ID : {Id}. {Message}", expenseId, ex.Message);
            }
            return mapper.Map<ExpenseReadOnlyDTO>(expense);
        }

        public async Task<List<ExpenseReadOnlyDTO>> GetExpensesByCategoryAsync(int userId, int categoryId)
        {
            // validate user exists
            var user = await unitOfWork.UserRepository.GetAsync(userId);
            if (user == null)
                throw new EntityNotFoundException("User", $"User with Id {userId} not found");

            // validate category exists
            var category = await unitOfWork.ExpenseCategoryRepository.GetAsync(categoryId);
            if (category == null)
                throw new EntityNotFoundException("ExpenseCategory", $"Category with Id {categoryId} not found");

            // Get all expenses from repository
            var expenses = await unitOfWork.ExpenseRepository
                .GetExpensesByCategoryAsync(userId, categoryId);

            // Map to DTOs
            var expenseDtos = mapper.Map<List<ExpenseReadOnlyDTO>>(expenses);

            // Fill category names
            foreach (var dto in expenseDtos)
                dto.Category = new ExpenseCategoryReadOnlyDTO { Id = category.Id, Name = category.Name };

            return expenseDtos;
        }

        public async Task<PaginatedResult<ExpenseReadOnlyDTO>> GetPaginatedUserExpensesAsync(int userId, int pageNumber, int pageSize)
        {
            //  validate user exists
            var user = await unitOfWork.UserRepository.GetAsync(userId);
            if (user == null)
                throw new EntityNotFoundException("User", $"User with Id {userId} not found");

            //paginated data
            var expenses = await unitOfWork.ExpenseRepository.GetPaginatedExpensesByUserAsync(userId, pageNumber, pageSize);

            //total count
            var totalCount = await unitOfWork.ExpenseRepository.GetCountByUserAsync(userId);

            var expenseDtos = mapper.Map<List<ExpenseReadOnlyDTO>>(expenses);

            //populate category info
            foreach (var dto in expenseDtos)
            {
                var category = expenses.FirstOrDefault(e => e.Id == dto.Id)?.ExpenseCategory;
                if (category != null)
                    dto.Category = new ExpenseCategoryReadOnlyDTO
                    {
                        Id = category.Id,
                        Name = category.Name
                    };
            }

            // Return paginated result
            return new PaginatedResult<ExpenseReadOnlyDTO>(
                data: expenseDtos,
                totalRecords: totalCount,
                pageNumber: pageNumber,
                pageSize: pageSize
            );

        }


        public async Task<decimal> GetTotalAmountByUserAsync(int userId)
        {
            var user = await unitOfWork.UserRepository.GetAsync(userId);
            if (user == null)
                throw new EntityNotFoundException("User", $"User with Id {userId} not found");

            // Get total amount
            var totalAmount = await unitOfWork.ExpenseRepository.GetTotalAmountByUserAsync(userId);
            return totalAmount;
        }

        public async Task<ExpenseReadOnlyDTO> UpdateExpenseAsync(int userId, int expenseId, ExpenseInsertDTO expenseDto)
        {
            //  find existing expense
            var existingExpense = await unitOfWork.ExpenseRepository.GetAsync(expenseId);
            if (existingExpense == null)
                throw new EntityNotFoundException("Expense", $"Expense with Id {expenseId} not found");

            // validate user ownership
            if (existingExpense.UserId != userId)
                throw new UnauthorizedAccessException("You can only update your own expenses.");

            // category check
            if (string.IsNullOrWhiteSpace(expenseDto.CategoryName))
                throw new InvalidArgumentException("Category", "Category name is required");

            var category = await unitOfWork.ExpenseCategoryRepository.GetByNameAsync(expenseDto.CategoryName.Trim());
            if (category == null)
            {
                category = new ExpenseCategory
                {
                    Name = expenseDto.CategoryName.Trim()
                };
                await unitOfWork.ExpenseCategoryRepository.AddAsync(category);
                await unitOfWork.SaveAsync(); // save to get new category Id
            }

            // Update expense fields
            existingExpense.Title = expenseDto.Title;
            existingExpense.Amount = expenseDto.Amount;
            existingExpense.Date = expenseDto.Date;
            existingExpense.ExpenseCategoryId = category.Id;

            await unitOfWork.ExpenseRepository.UpdateAsync(existingExpense);
            await unitOfWork.SaveAsync();

            var updatedExpenseDto = mapper.Map<ExpenseReadOnlyDTO>(existingExpense);
            updatedExpenseDto.Category = mapper.Map<ExpenseCategoryReadOnlyDTO>(category);
            return updatedExpenseDto;

        }


        public async Task<IEnumerable<ExpenseReadOnlyDTO>> SearchExpenseAsync(int userId, string searchTerm)
        {
            try
            {
                var user = await unitOfWork.UserRepository.GetAsync(userId)
                    ?? throw new EntityNotFoundException("User", $"User {userId} was not found");

                var expenses = await unitOfWork.ExpenseRepository.SearchByTitleAsync(userId, searchTerm);

                var result = mapper.Map<IEnumerable<ExpenseReadOnlyDTO>>(expenses);

                logger.LogInformation("Search found {Count} expenses for user with term '{Term}'", expenses.Count, userId, searchTerm);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Error searching Expenses. {Message}", ex.Message);
                throw;
            }
        }
    }
}
