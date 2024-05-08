using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersStudentsMVCApp.DTO;
using UsersStudentsMVCApp.Models;
using UsersStudentsMVCApp.Services;

namespace UsersStudentsMVCApp.Controllers
{
    public class UserController : Controller
    {
        public List<Error> ErrorArray { get; set; } = new();
        
        private readonly IApplicationService _applicationService;     

        public UserController(IApplicationService applicationService) 
            : base()
        {
            _applicationService = applicationService;
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserSignupDTO userSignupDTO)
        {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState.Values)
                {
                    foreach (var error in entry.Errors)
                    {
                        ErrorArray!.Add(new Error("", error.ErrorMessage, ""));
                    }                   
                }
                ViewData["ErrorArray"] = ErrorArray;
                return View(); ; // Return to the form with validation errors
            }

            try
            {
                await _applicationService.UserService.SignUpUserAsync(userSignupDTO);
                return RedirectToAction("Login", "User");
            }
            catch (Exception e)
            {
                ErrorArray!.Add(new Error("", e.Message, ""));
                ViewData["ErrorArray"] = ErrorArray;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            ClaimsPrincipal principal = HttpContext.User;
            if (principal.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(UserLoginDTO credentials)
        {
            var user = await _applicationService.UserService.VerifyAndGetUserAsync(credentials);
            if (user == null)
            {
                ViewData["ValidateMessage"] = "Error: User/Password not found ";
                return View();
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, credentials.Username!),
                new Claim(ClaimTypes.Role, user.UserRole!.ToString()!)
            };
            
            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new()
            {
                AllowRefresh = true,
                IsPersistent = credentials.KeepLoggedIn
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,                                           new ClaimsPrincipal(identity), properties);
            
            if (user.UserRole == UserRole.Teacher)
            {
                return RedirectToAction("Index", "Teacher");
            } else if (user.UserRole == UserRole.Student)
            {
                return RedirectToAction("Index", "Student");
            } else
            {
                return RedirectToAction("Index", "Admin");
            }
            //return RedirectToAction("Index", "Home");
        }
    }
}
