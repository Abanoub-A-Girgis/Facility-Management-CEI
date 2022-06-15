using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Crypto
{
    public class CryptoKycApplication : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("crypto-kyc-application")]
        public ActionResult cryptokycapplication()
        {
            return View();
        }
    }
}
