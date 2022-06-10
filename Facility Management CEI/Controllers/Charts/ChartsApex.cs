using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Charts
{
    public class ChartsApex : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("charts-apex")]
        public ActionResult chartsapex()
        {
            return View();
        }
    }
}
