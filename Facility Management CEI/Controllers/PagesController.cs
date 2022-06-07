using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class PagesController : Controller
    {
        // GET: Pages
        public ActionResult Page404()
        {
            return View();
        }

        public ActionResult Page500()
        {
            return View();
        }

        public ActionResult PageComingsoon()
        {
            return View();
        }

        public ActionResult PageFaqs()
        {
            return View();
        }

        public ActionResult PageMaintenance()
        {
            return View();
        }

        public ActionResult PagePricing()
        {
            return View();
        }

        public ActionResult PageStarter()
        {
            return View();
        }

        public ActionResult PageTimeline()
        {
            return View();
        }

    }
}