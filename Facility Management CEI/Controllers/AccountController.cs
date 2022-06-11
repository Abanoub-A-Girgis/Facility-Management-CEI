using Facility_Management_CEI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Facility_Management_CEI.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Register(RegisterViewModel model)
        {
            return View();
        }

        public IActionResult LogIn(LogInViewModel model)
        {
            return View();
        }

        public IActionResult LogOut()
        {
            return View();
        }
    }
}
