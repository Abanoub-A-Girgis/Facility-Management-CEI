using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Email
{
    public class EmailRead : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("email-read")]
        public ActionResult emailread()
        {
            return View();
        }
    }
}
