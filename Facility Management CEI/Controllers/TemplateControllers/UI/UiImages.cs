using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiImages : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-images")]
        public ActionResult uiimages()
        {
            return View();
        }
    }
}
