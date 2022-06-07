using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class ContactsController : Controller
    {
        // GET: Contacts
        public ActionResult ContactsGrid()
        {
            return View();
        }

        public ActionResult ContactsList()
        {
            return View();
        }

        public ActionResult ContactsProfile()
        {
            return View();
        }

    }
}