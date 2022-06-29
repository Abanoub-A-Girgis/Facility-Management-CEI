using Microsoft.AspNetCore.Mvc;

namespace Facility_Management_CEI.Controllers
{
    public class ErrorPagesController : Controller
    {
        public IActionResult Error404()
        {
            return View();
        }
    }
}
