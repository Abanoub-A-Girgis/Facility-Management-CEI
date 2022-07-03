using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiSessionTimeout : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-session-timeout")]
        public ActionResult uisessiontimeout()
        {
            return View();
        }
    }
}
