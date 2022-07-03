using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiColors : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-colors")]
        public ActionResult uicolors()
        {
            return View();
        }
    }
}
