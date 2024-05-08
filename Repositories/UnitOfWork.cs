using UsersStudentsMVCApp.Data;

namespace UsersStudentsMVCApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UsersTeachersTestDbContext _context;

        public UnitOfWork(UsersTeachersTestDbContext context)
        {
            _context = context;
        }

        public UserRepository UserRepository => new(_context);
        public StudentRepository StudentRepository => new(_context);
        public TeacherRepository TeacherRepository => new(_context);
        public CourseRepository CourseRepository => new(_context);


        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
