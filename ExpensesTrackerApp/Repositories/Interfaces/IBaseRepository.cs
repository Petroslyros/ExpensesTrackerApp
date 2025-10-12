namespace ExpensesTrackerApp.Repositories.Interfaces
{
    public interface IBaseRepository<T>

    {
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<T?> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> GetCountAsync();
    }
}
