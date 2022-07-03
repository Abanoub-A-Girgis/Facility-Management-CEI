using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Crypto
{
    public class CryptoExchange : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("crypto-exchange")]
        public ActionResult cryptoexchange()
        {
            return View();
        }
    }
}
