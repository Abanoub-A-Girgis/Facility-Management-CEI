using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Auth
{
    public class AuthRegister : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ActionName("auth-register")]
        public ActionResult authregister()
        {
            return View();
        }
    }
}
