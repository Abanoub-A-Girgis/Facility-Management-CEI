using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skote.Email
{
    public class EmailTemplateBasic : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
