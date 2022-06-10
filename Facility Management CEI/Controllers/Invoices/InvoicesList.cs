using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Invoices
{
    public class InvoicesList : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("invoices-list")]
        public ActionResult invoiceslist()
        {
            return View();
        }
    }
}
