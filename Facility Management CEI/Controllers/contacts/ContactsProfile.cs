using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.contacts
{
    public class ContactsProfile : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("contacts-profile")]
        public ActionResult contactsprofile()
        {
            return View();
        }
    }
}
