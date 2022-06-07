using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class TasksController : Controller
    {
        // GET: Tasks
        public ActionResult TaskCreate()
        {
            return View();
        }

        public ActionResult TaskKanban()
        {
            return View();
        }

        public ActionResult TaskList()
        {
            return View();
        }

    }
}