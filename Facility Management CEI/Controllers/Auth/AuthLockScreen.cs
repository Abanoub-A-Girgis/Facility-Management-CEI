using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Auth
{
    public class AuthLockScreen : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("auth-lock-screen")]
        public ActionResult authlockscreen()
        {
            return View();
        }
    }
}
