using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Auth
{
    public class AuthRecoverpw : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("auth-recoverpw")]
        public ActionResult authrecoverpw()
        {
            return View();
        }
    }
}
