using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Icon
{
    public class IconsMaterialDesign : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("icons-materialdesign")]
        public ActionResult iconsmaterialdesign()
        {
            return View();
        }
    }
}
