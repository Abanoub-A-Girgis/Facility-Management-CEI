using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Form
{
    public class FormRepeater : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("form-repeater")]
        public ActionResult formrepeater()
        {
            return View();
        }
    }
}
