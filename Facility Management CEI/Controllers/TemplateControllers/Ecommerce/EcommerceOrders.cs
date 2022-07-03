using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Ecommerce
{
    public class EcommerceOrders : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ecommerce-orders")]
        public ActionResult ecommerceorders()
        {
            return View();
        }
    }
}
