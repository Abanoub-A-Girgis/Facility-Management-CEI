using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.pages_404
{
    public class Pages404 : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        
        [ActionName("pages-404")]
        public ActionResult pages404()
        {
            return View();
        }
      
    }
}
