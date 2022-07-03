using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Layouts
{
    public class LayoutsColoredSidebar : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("layout-colored-sidebar")]
        public ActionResult layoutcoloredsidebar()
        {
            TempData["ModeName"] = Contants.LAYOUT_COLORED_SIDEBAR;
            TempData["WelcomeText"] = "LAYOUTS_COLORED_SIDEBAR";
            return RedirectToAction("Index", "Dashboard");
        }

    }
}
