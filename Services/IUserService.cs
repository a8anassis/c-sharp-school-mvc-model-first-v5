using UsersStudentsMVCApp.Data;
using UsersStudentsMVCApp.DTO;

namespace UsersStudentsMVCApp.Services
{
    public interface IUserService
    {
        Task SignUpUserAsync(UserSignupDTO request);
        Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials);
        Task<User?> UpdateUserAsync(int userId, UserDTO userDTO);
        Task<User?> UpdateUserPatchAsync(int userId, UserPatchDTO request);
        Task<User?> GetUserByUsernameAsync(string username);

        Task<List<User>> GetAllUsersFiltered(int pageNumber, int pageSize, UserFiltersDTO userFiltersDTO);
    }
}
