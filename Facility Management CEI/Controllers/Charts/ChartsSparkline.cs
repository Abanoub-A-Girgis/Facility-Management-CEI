using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Charts
{
    public class ChartsSparkline : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("charts-sparkline")]
        public ActionResult chartssparkline()
        {
            return View();
        }
    }
}
