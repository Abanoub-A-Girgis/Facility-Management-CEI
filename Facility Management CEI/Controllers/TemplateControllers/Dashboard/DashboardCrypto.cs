using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Dashboard
{
    public class DashboardCrypto : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("dashboard-crypto")]
        public ActionResult dashboardcrypto()
        {
            return View();
        }


    }
}
