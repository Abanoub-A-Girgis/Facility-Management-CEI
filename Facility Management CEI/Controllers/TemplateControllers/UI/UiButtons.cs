using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiButtons : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-buttons")]
        public ActionResult uibuttons()
        {
            return View();
        }
    }
}
