using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class MapsController : Controller
    {
        // GET: Maps
        public ActionResult MapGoogle()
        {
            return View();
        }

        public ActionResult MapLeaflet()
        {
            return View();
        }

        public ActionResult MapVector()
        {
            return View();
        }

    }
}