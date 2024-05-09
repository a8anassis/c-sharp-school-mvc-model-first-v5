using Microsoft.EntityFrameworkCore;
using UsersStudentsMVCApp.Data;
using UsersStudentsMVCApp.Security;

namespace UsersStudentsMVCApp.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(UsersTeachersTestDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Get the user based on username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The user, or null if username is invalid or password is invalid.</returns>
        public async Task<User?> GetUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username 
                    || x.Email == username);
            if (user == null)
            {
                return null;
            }
            if (!EncryptionUtil.IsValidPassword(password, user.Password!))
            {
                return null;
            }
            return user;
        }

        /// <summary>
        /// Updates User.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User?> UpdateUserAsync(int userId, User user)
        {
            // Optimized Query - First Where, then FirstAsync
            var existingUser = await _context.Users.Where(x => x.Id == userId)
                .FirstOrDefaultAsync();

            // Not optimized - just searches
            //var user = await _context.Users.FirstAsync(x => x.Id == userId);
            if (existingUser is null) return null;
            if (existingUser.Id != userId) return null;

            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;

            return existingUser;
        }

        /// <summary>
        /// Returns the user based on his username. 
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The user if the username exists, null otherwise.</returns>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            var user = await _context.Users.Where(x => x.Username == username)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<List<User>> GetAllUsersFilteredAsync(int pageNumber, int pageSize, 
            List<Func<User, bool>> predicates)
        {
            int skip = pageSize * pageNumber;
            IQueryable<User> query = _context.Users.Skip(skip).Take(pageSize);

            // Combine multiple predicates using && operator
            if (predicates != null && predicates.Any())
            {
                query = query.Where(u => predicates.All(predicate => predicate(u)));
            }

            return await query.ToListAsync();
        }
    }
}
