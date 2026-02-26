using AutoMapper;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.DTO;
using ExpensesTrackerApp.Repositories.Interfaces;
using ExpensesTrackerApp.Services.Interfaces;
using Serilog;

namespace ExpensesTrackerApp.Services
{
    public class ExpenseCategoryService : IExpenseCategoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<ExpenseCategoryService> logger = new LoggerFactory().AddSerilog().CreateLogger<ExpenseCategoryService>();

        public ExpenseCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public Task<ExpenseCategoryReadOnlyDTO> CreateExpenseCategoryAsync(ExpenseCategoryInsertDTO newCategory)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExpenseCategoryReadOnlyDTO>> GetAllExpenseCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ExpenseCategory?> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<ExpenseCategoryReadOnlyDTO?> GetExpenseCategoryByIdAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<ExpenseCategoryReadOnlyDTO?> GetExpenseCategoryByNameAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
