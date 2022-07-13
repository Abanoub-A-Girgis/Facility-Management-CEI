using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Tables
{
    public class TablesEditable : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("tables-editable")]
        public ActionResult tableseditable()
        {
            return View();
        }
    }
}
