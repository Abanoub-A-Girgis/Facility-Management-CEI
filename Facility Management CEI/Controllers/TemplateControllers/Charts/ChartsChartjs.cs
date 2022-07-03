using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Charts
{
    public class ChartsChartjs : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("charts-chartjs")]
        public ActionResult chartschartjs()
        {
            return View();
        }
    }
}
