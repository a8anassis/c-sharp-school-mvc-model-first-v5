using Microsoft.EntityFrameworkCore;
using UsersStudentsMVCApp.Data;
using UsersStudentsMVCApp.Models;

namespace UsersStudentsMVCApp.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(UsersTeachersTestDbContext context)
            : base(context)
        {
        }

        public async Task<Student?> GetByPhoneNumber(string? phoneNumber)
        {
            return await _context.Students.Where(s => s.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync()!;  
        }

        public async Task<List<Course>> GetStudentCoursesAsync(int id)
        {
            List<Course> courses;
            courses = await _context.Students
                       .Where(s => s.Id == id)
                       .SelectMany(s => s.Courses!)
                       .ToListAsync();
            return  courses;
        }

        public async Task<List<User>> GetAllUsersStudentsAsync()
        {
            var usersWithStudentRole = await _context.Users
                   .Where(u => u.UserRole == UserRole.Student) // Assuming UserRole is the enum for roles
                   .Include(u => u.Student) // Assuming Student is the navigation property representing the related Student entity
                   .ToListAsync();

            return usersWithStudentRole;
        }
    }
}
