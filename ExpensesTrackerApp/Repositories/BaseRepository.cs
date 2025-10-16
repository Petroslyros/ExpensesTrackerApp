using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesTrackerApp.Repositories
{
    /// <summary>
    /// Provides a generic implementation of common data access operations for entities of type <typeparamref name="T"/>.
    /// Serves as a base class for specific repositories, leveraging Entity Framework Core.
    /// </summary>
    /// <typeparam name="T">The entity type managed by the repository.</typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ExpenseAppDbContext context;
        protected readonly DbSet<T> dbSet; //represents DB table and handles SQL queries

        public BaseRepository(ExpenseAppDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        #region docs
        /// <summary>
        /// Asynchronously adds a single entity to the database context.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        #endregion
        public virtual async Task AddAsync(T entity) => await dbSet.AddAsync(entity);

        #region docs
        /// <summary>
        /// Asynchronously adds a collection of entities to the database context.
        /// </summary>
        /// <param name="entity">The entities to add.</param>
        /// <returns></returns>
        #endregion
        public virtual async Task AddRangeAsync(IEnumerable<T> entities) => await dbSet.AddRangeAsync(entities);



        #region docs
        /// <summary>
        /// Asynchronously retrieves all entities of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A collection of all entities.</returns>
        #endregion
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();

        #region docs
        /// <summary>
        /// Asynchronously retrieves an entity by its primary key.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The matching entity, or null if not found.</returns>
        #endregion
        public virtual async Task<T?> GetAsync(int id) => await dbSet.FindAsync(id);

        #region docs
        /// <summary>
        /// Asynchronously counts the total number of entities of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The total count of entities.</returns>
        #endregion
        public virtual async Task<int> GetCountAsync() => await dbSet.CountAsync();

        #region docs
        /// <summary>
        /// Marks an entity as modified in the database context to prepare it for update.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        #endregion
        public Task UpdateAsync(T entity)   //Not async, 
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        #region docs
        /// <summary>
        /// Asynchronously deletes an entity by its primary key.
        /// Returns true if the entity was found and removed; otherwise, false.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>True if deletion was successful; otherwise, false.</returns>
        #endregion
        public virtual async Task<bool> DeleteAsync(int id)
        {
            T? existingEntity = await GetAsync(id);
            if (existingEntity is null) return false;
            dbSet.Remove(existingEntity);
            return true;
        }
    }
}
