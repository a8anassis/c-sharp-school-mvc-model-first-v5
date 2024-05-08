using UsersStudentsMVCApp.Data;

namespace UsersStudentsMVCApp.Repositories
{
    public interface ICourseRepository
    {
        Task<List<Student>> GetCourseStudentsAsync(int id);
        Task<Teacher?> GetCourseTeacherAsync(int id);
    }
}
