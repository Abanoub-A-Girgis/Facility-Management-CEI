using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Crypto
{
    public class CryptoLending : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("crypto-lending")]
        public ActionResult cryptolending()
        {
            return View();
        }
    }
}
