using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class ProjectsController : Controller
    {
        // GET: Projects
        public ActionResult ProjectCreate()
        {
            return View();
        }

        public ActionResult ProjectGrid()
        {
            return View();
        }

        public ActionResult ProjectList()
        {
            return View();
        }

        public ActionResult ProjectOverview()
        {
            return View();
        }

    }
}