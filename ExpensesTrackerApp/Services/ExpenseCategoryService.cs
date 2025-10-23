using AutoMapper;
using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Repositories.Interfaces;
using Serilog;

namespace ExpensesTrackerApp.Services
{
    public class ExpenseCategoryService : IExpenseCategoryRepository
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<ExpenseCategoryService> logger = new LoggerFactory().AddSerilog().CreateLogger<ExpenseCategoryService>();

        public ExpenseCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public Task<ExpenseCategory?> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
