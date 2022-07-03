using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Layouts
{
    public class LayoutsScrollable : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
