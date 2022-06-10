using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiLightbox : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-lightbox")]
        public ActionResult uilightbox()
        {
            return View();
        }
    }
}
