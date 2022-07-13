using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Maps
{
    public class MapsVector : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("maps-vector")]
        public ActionResult mapsvector()
        {
            return View();
        }
    }
}
