using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skote.Layouts
{
    public class LayoutHoriPreloader : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
