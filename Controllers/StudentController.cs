using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace UsersStudentsMVCApp.Controllers
{
    public class StudentController : Controller
    {
        [Authorize(Roles = "Student")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
