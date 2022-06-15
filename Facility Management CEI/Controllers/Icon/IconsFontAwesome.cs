using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Icon
{
    public class IconsFontAwesome : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("icons-fontawesome")]
        public ActionResult iconsfontawesome()
        {
            return View();
        }
    }
}
