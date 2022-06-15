using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Form
{
    public class FormEditors : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("form-editors")]
        public ActionResult formeditors()
        {
            return View();
        }
    }
}
