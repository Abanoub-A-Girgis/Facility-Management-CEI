using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Layouts
{
    public class LayoutsCompactSidebar : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("layout-compact-sidebar")]
        public ActionResult layoutcompactsidebar()
        {
            TempData["ModeName"] = Contants.LAYOUT_COMPACT_SIDEBAR;
            TempData["WelcomeText"] = "LAYOUT_COMPACT_SIDEBAR";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
