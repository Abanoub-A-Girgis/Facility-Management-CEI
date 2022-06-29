using Microsoft.AspNetCore.Mvc;

namespace Facility_Management_CEI.Controllers
{
    public class ErrorPagesController : Controller
    {
        public IActionResult Error404()
        {
            return View();
        }
        public IActionResult ErrorGeneric()
        {
            if (TempData.ContainsKey("message"))
            {
                var message = TempData["message"];
                return View(message);
            }
            else
            {
                string message = "No Error Message";
                return View((object)message);
            }
        }
    }
}
