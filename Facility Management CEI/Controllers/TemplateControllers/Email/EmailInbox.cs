using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Email
{
    public class EmailInbox : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("email-inbox")]
        public ActionResult emailinbox()
        {
            return View();
        }
    }
}
