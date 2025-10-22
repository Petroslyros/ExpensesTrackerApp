using AutoMapper;
using ExpensesTrackerApp.Repositories.Interfaces;
using ExpensesTrackerApp.Services.Interfaces;

namespace ExpensesTrackerApp.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public UserService UserService => new(unitOfWork, mapper);

        public ExpenseService ExpenseService => new(unitOfWork, mapper);

        public ExpenseCategoryService ExpenseCategoryService => new(unitOfWork, mapper);

        public BudgetService BudgetService => new(unitOfWork, mapper);
    }
}
