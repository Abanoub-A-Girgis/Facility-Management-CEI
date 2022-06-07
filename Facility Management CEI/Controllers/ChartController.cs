using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class ChartController : Controller
    {
        // GET: Chart
        public ActionResult Apex()
        {
            return View();
        }

        public ActionResult Chartjs()
        {
            return View();
        }

        public ActionResult Echart()
        {
            return View();
        }

        public ActionResult Flot()
        {
            return View();
        }

        public ActionResult Knob()
        {
            return View();
        }

        public ActionResult Sparkline()
        {
            return View();
        }

        public ActionResult ChartTui()
        {
            return View();
        }

    }
}