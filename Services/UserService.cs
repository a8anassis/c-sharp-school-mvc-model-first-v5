using AutoMapper;
using System.Reflection;
using UsersStudentsMVCApp.Data;
using UsersStudentsMVCApp.DTO;
using UsersStudentsMVCApp.Repositories;
using UsersStudentsMVCApp.Security;
using UsersStudentsMVCApp.Services.Exceptions;
using static System.Net.Mime.MediaTypeNames;

namespace UsersStudentsMVCApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork? _unitOfWork;       
        private readonly ILogger<UserService>? _logger;
        private readonly IMapper? _mapper;

        public UserService(IUnitOfWork? unitOfWork, ILogger<UserService>? logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;           
            _logger = logger;  
            _mapper = mapper;
        }

        public async Task SignUpUserAsync(UserSignupDTO signupDTO)
        {            
            Student? student;
            Teacher? teacher;
            User? user;

            try
            {
                user = ExtractUser(signupDTO);
                User? existingUser = await _unitOfWork!.UserRepository
                    .GetByUsernameAsync(user.Username!);

                if ( existingUser != null)
                {
                    throw new UserAlreadyExistsException("UserExists: " + existingUser.Username);
                }

                user.Password = EncryptionUtil.Encrypt(user.Password!);
                await _unitOfWork!.UserRepository.AddAsync(user);

                if (user!.UserRole!.Value.ToString().Equals("Student"))
                {
                    student = ExtractStudent(signupDTO);
                    if (await _unitOfWork!.StudentRepository.GetByPhoneNumber(student.PhoneNumber) is not null)
                    {
                        throw new StudentAlreadyExistsException("StudentExists");
                    }
                    await _unitOfWork!.StudentRepository.AddAsync(student);
                    user.Student = student;
                    // student.User = user; Since entities are attached EF manages the other end of the relationship
                }
                else if (user!.UserRole!.Value.ToString().Equals("Teacher"))
                {
                    teacher = ExtractTeacher(signupDTO);
                    if (await _unitOfWork!.TeacherRepository.GetByPhoneNumber(teacher.PhoneNumber) is not null)
                    {
                        throw new TeacherAlreadyExistsException("StudentExists");
                    }
                    await _unitOfWork!.TeacherRepository.AddAsync(teacher);
                    user.Teacher = teacher;
                    // teacher.User = user;   // EF manages the other end since both entities are attached 
                }
                else
                {
                    throw new InvalidRoleException("InvalidRole");                 
                }
                
                await _unitOfWork!.SaveAsync();
                _logger!.LogInformation("{Message}", "User: " + user + " signup success");
            } catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }           
        }

        public async Task<User?> VerifyAndGetUserAsync(UserLoginDTO request)
        {
            User? user = null;

            try
            {
                user = await _unitOfWork!.UserRepository.GetUserAsync(request.Username!, request.Password!);
                _logger!.LogInformation("{Message}", "User: " + user + " found and returned");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }           
            return user;
        }

        public async Task<User?> UpdateUserAsync(int userId, UserDTO userDTO)
        {
            User? existingUser;
            User? user = null;
            try
            {
                existingUser = await _unitOfWork!.UserRepository.GetAsync(userId);
                if (existingUser == null) return null;

                var userToUpdate = _mapper!.Map<User>(userDTO);

                user = await _unitOfWork.UserRepository.UpdateUserAsync(userId, userToUpdate);
                await _unitOfWork.SaveAsync();
                _logger!.LogInformation("{Message}", "User: " + user + " updated successfully");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }
            return user;
        }

        public async Task<User?> UpdateUserPatchAsync(int userId, UserPatchDTO userPatchDTO)
        {
            User? existingUser;
            User? user = null;
            try
            {
                existingUser = await _unitOfWork!.UserRepository.GetAsync(userId);
                if (existingUser == null) return null;

                existingUser.Username = userPatchDTO.Username;
                existingUser.Email = userPatchDTO.Email;
                existingUser.Password = EncryptionUtil.Encrypt(userPatchDTO.Password!);

                // No need for Update command. Since existing user is attached
                // EF will detect changes and provide an update SQL statement.
                // The update statement will include only the modified columns.
                await _unitOfWork.SaveAsync();
                _logger!.LogInformation("{Message}", "User: " + user + " updated successfully");
            } catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }
            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            User? user = null;
            try
            {
                user = await _unitOfWork!.UserRepository.GetByUsernameAsync(username);
                _logger!.LogInformation("{Message}", "User: " + user + " found and returned" );
            } catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }
            return user;
        }

        public async Task<List<User>> GetAllUsersFiltered(int pageNumber, int pageSize, UserFiltersDTO userFiltersDTO)
        {
            List<User> users = new();
            List<Func<User, bool>> predicates = new();

            // Add individual predicates for filtering conditions
            if (!string.IsNullOrEmpty(userFiltersDTO.Username))
            {
                predicates.Add(u => u.Username == userFiltersDTO.Username);
            }
            if (!string.IsNullOrEmpty(userFiltersDTO.Email))
            {
                predicates.Add(u => u.Email == userFiltersDTO.Email);
            }
            if (!string.IsNullOrEmpty(userFiltersDTO.UserRole))
            {
                predicates.Add(u => u.UserRole!.Value.ToString() == userFiltersDTO.UserRole);
            }

            users = await _unitOfWork!.UserRepository.GetAllUsersFilteredAsync(pageNumber, pageSize, 
                predicates);
            return users;
        }

        private User ExtractUser(UserSignupDTO signupDTO)
        {
            return new User()
            {
                Username = signupDTO.Username,
                Password = signupDTO.Password,
                Email = signupDTO.Email,
                Firstname = signupDTO.Firstname,
                Lastname = signupDTO.Lastname,
                UserRole = signupDTO.UserRole
            };
        }

        private Student ExtractStudent(UserSignupDTO? signupDTO)
        {
            return new Student()
            {
                PhoneNumber = signupDTO!.PhoneNumber!,
                Institution = signupDTO!.Institution,
            };
        }

        private Teacher ExtractTeacher(UserSignupDTO? signupDTO)
        {
            return new Teacher()
            {
                PhoneNumber = signupDTO!.PhoneNumber!,
                Institution = signupDTO!.Institution,
            };
        }
    }
}
