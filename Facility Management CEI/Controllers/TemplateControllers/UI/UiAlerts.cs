using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiAlerts : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-alerts")]
        public ActionResult uialerts()
        {
            return View();
        }
        
    }
}
