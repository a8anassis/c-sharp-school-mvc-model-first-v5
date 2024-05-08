using AutoMapper;
using UsersStudentsMVCApp.Data;
using UsersStudentsMVCApp.DTO;

namespace UsersStudentsMVCApp.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<User, UserPatchDTO>().ReverseMap();
            CreateMap<User, UserSignupDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
