using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Skote.Controllers
{
    public class FileManagerController : Controller
    {
        // GET: FileManager
        public ActionResult Index()
        {
            return View();
        }
    }
}