using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class TablesController : Controller
    {
        // GET: Tables
        public ActionResult TableBasic()
        {
            return View();
        }

        public ActionResult TableDatatable()
        {
            return View();
        }

        public ActionResult TableEditable()
        {
            return View();
        }

        public ActionResult TableResponsive()
        {
            return View();
        }

    }
}