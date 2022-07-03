using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Auth
{
    public class AuthLogin : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ValidateLogin(string email, string password)
        {
            //You can fetch email and password from db or API here.
            string dbEmail = "Test";
            string dbPassword = "123";
            bool IsValidUser = false;

            if (email == dbEmail && password == dbPassword)
                IsValidUser = true;

            return Json(new
            {
                IsValidUser = IsValidUser
            });
        }
    }
}
