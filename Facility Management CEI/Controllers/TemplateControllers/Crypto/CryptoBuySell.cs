using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Crypto
{
    public class CryptoBuySell : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("crypto-buy-sell")]
        public ActionResult cryptobuysell()
        {
            return View();
        }
    }
}
