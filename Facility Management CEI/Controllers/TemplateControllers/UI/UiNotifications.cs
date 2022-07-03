using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiNotifications : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-notifications")]
        public ActionResult uinotifications()
        {
            return View();
        }
    }
}
