using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace Skote.Projects
{
    public class ProjectsCreate : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("projects-create")]
        public ActionResult projectscreate()
        {
            return View();
        }

    }
}
