using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Charts
{
    public class ChartsEchart : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("charts-echart")]
        public ActionResult chartsechart()
        {
            return View();
        }
    }
}
