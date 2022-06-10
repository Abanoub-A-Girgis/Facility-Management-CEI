using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiVideo : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-video")]
        public ActionResult uivideo()
        {
            return View();
        }

    }
}
