using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Ecommerce
{
    public class EcommerceAddProduct : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ecommerce-addproduct")]
        public ActionResult ecommerceaddproduct()
        {
            return View();
        }
    }
}
