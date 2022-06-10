using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Tables
{
    public class TablesDatatable : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("tables-datatable")]
        public ActionResult tablesdatatable()
        {
            return View();
        }
    }
}
