using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Dashboard
{
    public class DashboardSaas : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("dashboard-saas")]
        public ActionResult dashboardsaas()
        {
            return View();
        }
    }
}
