using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Ecommerce
{
    public class EcommerceShops : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ecommerce-shops")]
        public ActionResult ecommerceshops()
        {
            return View();
        }
    }
}
