using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Form
{
    public class FormValidation : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("form-validation")]
        public ActionResult formvalidation()
        {
            return View();
        }
    }
}
