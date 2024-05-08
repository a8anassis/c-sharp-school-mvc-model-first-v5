using System;
using UsersStudentsMVCApp.Data;
using UsersStudentsMVCApp.DTO;

namespace UsersStudentsMVCApp.Repositories
{
    public interface IUserRepository
    {
        //Task<int?> SignUpUserAsync(User request);
        //Task SignUpUserAsync(User request);
        Task<User?> GetUserAsync(string username, string password);
        Task<User?> UpdateUserAsync(int userId, User request);
        Task<User?> GetByUsernameAsync(string username);
        Task<List<User>> GetAllUsersFilteredAsync(int pageNumber, int pageSize, List<Func<User, bool>> predicates);
    }
}
