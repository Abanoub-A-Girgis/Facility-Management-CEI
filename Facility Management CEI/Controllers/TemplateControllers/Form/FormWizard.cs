using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Form
{
    public class FormWizard : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("form-wizard")]
        public ActionResult formwizard()
        {
            return View();
        }
    }
}
