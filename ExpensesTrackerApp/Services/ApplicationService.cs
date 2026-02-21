using AutoMapper;
using ExpensesTrackerApp.Repositories.Interfaces;
using ExpensesTrackerApp.Services.Interfaces;

namespace ExpensesTrackerApp.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILoggerFactory loggerFactory;

        public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper, ILoggerFactory loggerFactory)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.loggerFactory = loggerFactory;
        }
        public UserService UserService => new(unitOfWork, mapper, loggerFactory.CreateLogger<UserService>());

        public ExpenseService ExpenseService => new(unitOfWork, mapper, loggerFactory.CreateLogger<ExpenseService>());

        public ExpenseCategoryService ExpenseCategoryService => new(unitOfWork, mapper);

        public BudgetService BudgetService => new(unitOfWork, mapper, loggerFactory.CreateLogger<BudgetService>());
    }
}
