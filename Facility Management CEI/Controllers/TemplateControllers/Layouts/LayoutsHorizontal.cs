using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Layouts
{
    public class LayoutsHorizontal : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("layout-horizontal")]
        public ActionResult layouthorizontal()
        {
            TempData["ModeName"] = Contants.LAYOUT_HORIZONTAL;
            TempData["WelcomeText"] = "LAYOUT_HORIZONTAL";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
