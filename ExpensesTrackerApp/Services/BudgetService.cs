using AutoMapper;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Exceptions;
using ExpensesTrackerApp.Repositories.Interfaces;
using ExpensesTrackerApp.Services.Interfaces;

namespace ExpensesTrackerApp.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<BudgetService> logger;

        public BudgetService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BudgetService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<BudgetReadOnlyDTO> CreateBudgetAsync(int userId, BudgetInsertDTO dto)
        {
            //validate user
            var user = await unitOfWork.UserRepository.GetAsync(userId);
            if (user == null)
                throw new EntityNotFoundException("User", $"User with Id {userId} not found");

            //validate category name
            var category = await unitOfWork.ExpenseCategoryRepository.GetAsync(dto.CategoryId);
            if (category == null)
                throw new EntityNotFoundException("ExpenseCategory", $"Category with Id {dto.CategoryId} not found");

            //check if category + user already exists
            var existingBudget = await unitOfWork.BudgetRepository.GetBudgetByUserAndCategoryAsync(userId, dto.CategoryId);
            if (existingBudget != null)
                throw new EntityAlreadyExistsException("Budget", "A budget already exists for this user and category");

            //Map DTO to Entity
            var budget = mapper.Map<Budget>(dto);
            budget.UserId = userId;
            budget.CategoryId = dto.CategoryId;
            budget.SpentAmount = 0;

            await unitOfWork.BudgetRepository.AddAsync(budget);
            await unitOfWork.SaveAsync();

            var result = mapper.Map<BudgetReadOnlyDTO>(budget);
            result.CategoryName = category.Name;

            return result;
        }

        public Task DeleteBudgetAsync(int budgetId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BudgetReadOnlyDTO>> GetBudgetsByUserAsync(int userId)
        {
            try
            {
                var budgets = await unitOfWork.BudgetRepository.GetBudgetsByUserAsync(userId);
                var result = mapper.Map<IEnumerable<BudgetReadOnlyDTO>>(budgets);

                logger.LogInformation("Retrieved {Count} budgets for user {UserId}", result.Count(), userId);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("Error retrieving budgets for user {UserId}. {Message}", userId, ex.Message);
                throw;
            }
        }

        public async Task<BudgetReadOnlyDTO?> GetBudgetByIdAsync(int budgetId)
        {
            Budget? budget = null;
            try
            {
                budget = await unitOfWork.BudgetRepository.GetAsync(budgetId);
                logger.LogInformation("Budget found with ID : {Id}", budgetId);
            }
            catch (Exception ex)
            {
                logger.LogError("Error retreiving Budget with ID : {Id}. {Message}", budgetId, ex.Message);
            }
            return mapper.Map<BudgetReadOnlyDTO>(budget);
        }

        public async Task DeleteBudgetAsync(int budgetId, int userId)
        {

            var budget = await unitOfWork.BudgetRepository.GetAsync(budgetId);

            if (budget == null)
                throw new EntityNotFoundException("Budget", $"Budget with ID {budgetId} not found");

            // Verify ownership
            if (budget.UserId != userId)
                throw new EntityNotAuthorizedException("Budget", "You don't have permission to delete this budget");

            // Delete
            await unitOfWork.BudgetRepository.DeleteAsync(budgetId);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Budget deleted successfully: {BudgetId}", budgetId);
        }
    }
}
