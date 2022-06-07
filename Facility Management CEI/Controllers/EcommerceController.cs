using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class EcommerceController : Controller
    {
        // GET: Ecommerce
        public ActionResult EcommerceAddProduct()
        {
            return View();
        }

        public ActionResult EcommerceCart()
        {
            return View();
        }

        public ActionResult EcommerceCheckout()
        {
            return View();
        }

        public ActionResult EcommerceCustomers()
        {
            return View();
        }

        public ActionResult EcommerceOrders()
        {
            return View();
        }

        public ActionResult EcommerceProductDetail()
        {
            return View();
        }

        public ActionResult EcommerceProduct()
        {
            return View();
        }

        public ActionResult EcommerceShops()
        {
            return View();
        }

    }
}