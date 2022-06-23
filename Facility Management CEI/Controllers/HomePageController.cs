using Microsoft.AspNetCore.Mvc;

namespace Facility_Management_CEI.Controllers
{
    public class HomePageController : Controller
    {
        public IActionResult HomePage()
        {
            return View();
        }
    }
}
