using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiSweetAlert : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-sweet-alert")]
        public ActionResult uisweetalert()
        {
            return View();
        }
    }
}
