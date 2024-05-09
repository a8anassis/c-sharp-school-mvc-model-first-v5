using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersStudentsMVCApp.Data;
using UsersStudentsMVCApp.DTO;
using UsersStudentsMVCApp.Models;
using UsersStudentsMVCApp.Services;

namespace UsersStudentsMVCApp.Controllers
{
    public class AdminController : Controller {

        public List<UserDTO>? UsersDTO { get; set; } = new();
        public List<Error> ErrorArray { get; set; } = new();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;


        private readonly IMapper? _mapper;
        private readonly IApplicationService _applicationService;

        public AdminController(IApplicationService applicationService)
            : base()
        {
            _applicationService = applicationService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(UserFiltersDTO userFiltersDTO)
        {
            int pageSize = PageSize;
            int pageNumber = PageNumber;

            try
            {
                _ = int.TryParse(Request.Query["pagenumber"], out pageNumber);
                _ = int.TryParse(Request.Query["pagesize"], out pageSize); 

                List<User> users = await _applicationService.UserService.GetAllUsersFiltered(pageNumber, 
                    pageSize, userFiltersDTO);
                
                foreach (var user in users)
                {
                    UserDTO? userDTO = _mapper!.Map<UserDTO>(user);
                    UsersDTO!.Add(userDTO);
                }
            }
            catch (Exception e)
            {
                ErrorArray = new() { 
                    new Error("", e.Message, "")
                };
            }

            return View();
        }
    }
    
}
