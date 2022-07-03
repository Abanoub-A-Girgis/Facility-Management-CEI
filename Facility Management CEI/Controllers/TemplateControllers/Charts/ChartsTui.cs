using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Charts
{
    public class ChartsTui : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("charts-tui")]
        public ActionResult chartstui()
        {
            return View();
        }
    }
}
