using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Dashboard
{
    public class DashboardBlog : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("dashboard-blog")]
        public ActionResult dashboardblog()
        {
            return View();
        }


    }
}
