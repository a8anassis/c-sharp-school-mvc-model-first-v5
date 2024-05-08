using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using UsersStudentsMVCApp.Models;

namespace UsersStudentsMVCApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ClaimsPrincipal principal = HttpContext.User;
            // Get all role claims
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var roleClaims = principal.Claims.Where(c => c.Type == ClaimTypes.Role);

            if (userIdClaim == null)
            {
                Console.WriteLine("Principal is NULL");
                return View();
            }
            string userId = userIdClaim!.Value;
            Console.WriteLine("User ID: " + userId);

            // Print the role claims
            foreach (var roleClaim in roleClaims)
            {
                Console.WriteLine("Role: " + roleClaim.Value);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}