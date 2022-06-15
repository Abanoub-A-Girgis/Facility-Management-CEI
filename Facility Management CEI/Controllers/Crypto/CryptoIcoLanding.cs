using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Crypto
{
    public class CryptoIcoLanding : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("crypto-ico-landing")]
        public ActionResult cryptoicolanding()
        {
            return View();
        }
    }
}
