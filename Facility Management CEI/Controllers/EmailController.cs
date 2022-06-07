using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult EmailInbox()
        {
            return View();
        }

        public ActionResult EmailRead()
        {
            return View();
        }

        public ActionResult EmailTemplateAlert()
        {
            return View();
        }

        public ActionResult EmailTemplateBasic()
        {
            return View();
        }

        public ActionResult EmailTemplateBilling()
        {
            return View();
        }

    }
}