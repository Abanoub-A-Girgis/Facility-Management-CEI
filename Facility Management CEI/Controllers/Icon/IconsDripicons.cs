using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Icon
{
    public class IconsDripicons : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("icons-dripicons")]
        public ActionResult iconsdripicons()
        {
            return View();
        }
    }
}
