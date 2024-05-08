using UsersStudentsMVCApp.Data;

namespace UsersStudentsMVCApp.Repositories
{
    public interface IStudentRepository
    {
        Task<List<Course>> GetStudentCoursesAsync(int id);
        Task<Student?> GetByPhoneNumber(string? phoneNumber);
        Task<List<User>> GetAllUsersStudentsAsync();
    }
}
