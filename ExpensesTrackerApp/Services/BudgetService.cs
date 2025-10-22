using AutoMapper;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Exceptions;
using ExpensesTrackerApp.Repositories.Interfaces;
using ExpensesTrackerApp.Services.Interfaces;
using Serilog;

namespace ExpensesTrackerApp.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<ExpenseService> logger = new LoggerFactory().AddSerilog().CreateLogger<ExpenseService>();

        public BudgetService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
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

        public Task<IEnumerable<BudgetReadOnlyDTO>> GetBudgetsByUserAsync(int userId)
        {
            throw new NotImplementedException();
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
    }
}
