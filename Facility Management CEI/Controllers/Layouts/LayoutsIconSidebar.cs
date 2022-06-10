using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Layouts
{
    public class LayoutsIconSidebar : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("layout-icon-sidebar")]
        public ActionResult layouticonsidebar()
        {
            TempData["ModeName"] = Contants.LAYOUT_ICON_SIDEBAR;
            TempData["WelcomeText"] = "LAYOUT_ICON_SIDEBAR";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
