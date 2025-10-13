using ExpensesTrackerApp.DTO;

namespace ExpensesTrackerApp.Services.Interfaces
{
    public interface IExpenseCategoryService
    {
        Task<ExpenseCategoryReadOnlyDTO?> GetExpenseCategoryByIdAsync(int categoryId);

        Task<ExpenseCategoryReadOnlyDTO?> GetExpenseCategoryByNameAsync(string name);

        Task<ExpenseCategoryReadOnlyDTO> CreateExpenseCategoryAsync(ExpenseCategoryInsertDTO newCategory);

        Task<List<ExpenseCategoryReadOnlyDTO>> GetAllExpenseCategoriesAsync();
    }
}
