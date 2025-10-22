using ExpensesTrackerApp.Data;
using ExpensesTrackerApp.Models;
using ExpensesTrackerApp.Repositories.Interfaces;
using ExpensesTrackerApp.Security;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExpensesTrackerApp.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ExpenseAppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserAsync(string username, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username
            || u.Email == username);

            if (user == null) return null;

            if (!EncryptionUtil.IsValidPassword(password, user.Password)) return null;

            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username) =>
            await context.Users.FirstOrDefaultAsync(u => u.Username == username);


        public async Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates)
        {
            IQueryable<User> query = context.Users.Where(u => !u.isDeleted);// δεν εκτελείται

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate); // εκτελείται, υπονοείται το AND
                }
            }

            int totalRecords = await query.CountAsync(); // εκτελείται

            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Id) // πάντα να υπάρχει ένα OrderBy πριν το Skip
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(); // εκτελείται

            return new PaginatedResult<User>(data, totalRecords, pageNumber, pageSize);
        }


        #region docs
        /// <summary>
        /// Checks whether a user with the specified email already exists.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns><c>true</c> if the email exists; otherwise, <c>false</c>.</returns>
        #endregion
        public async Task<bool> EmailExistsAsync(string? email) => await context.Users.AnyAsync(u => u.Email == email);




        #region docs
        /// <summary>
        /// Updates an existing user with new data.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="user">The updated <see cref="User"/> object.</param>
        /// <returns>
        /// The original <see cref="User"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        #endregion
        public async Task<User?> UpdateAsync(int id, User user)
        {
            var existingUser = await context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            context.Users.Attach(user);
            context.Entry(user).State = EntityState.Modified;  // Marks the UserProfile entity as modified

            return existingUser;
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await dbSet.Where(u => !u.isDeleted).ToListAsync();
        }



    }
}
