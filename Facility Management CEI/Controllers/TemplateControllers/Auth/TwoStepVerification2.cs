using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skote.Auth
{
    public class TwoStepVerification2 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
