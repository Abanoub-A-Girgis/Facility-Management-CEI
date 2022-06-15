using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiGeneral : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-general")]
        public ActionResult uigeneral()
        {
            return View();
        }
    }
}
