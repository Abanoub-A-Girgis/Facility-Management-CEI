using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Ecommerce
{
    public class EcommerceCheckout : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ecommerce-checkout")]
        public ActionResult ecommercecheckout()
        {
            return View();
        }
    }
}
