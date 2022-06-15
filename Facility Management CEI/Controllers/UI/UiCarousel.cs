using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiCarousel : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-carousel")]
        public ActionResult uicarousel()
        {
            return View();
        }
    }
}
