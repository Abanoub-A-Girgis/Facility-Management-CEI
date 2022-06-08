using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{

    //authorization is made at the entire controller as only the owner system admin manger will see this report 
    // the opposit of [Authorize] is [allow anynoumous] which will let any one to access to you controller
    //[AllowAnonymous] bypasses all authorization statements. If you combine [AllowAnonymous] and any [Authorize] attribute, the [Authorize] attributes are ignored.
    //[Authorize]//this is writtin to make sure this is not public and requires autorization 
    public class ChartController : Controller
    {
        public ApplicationDBContext _context { get; set; }
        public ChartController(ApplicationDBContext context)
        {
            _context = context;
        }
        // GET: Chart
        public async Task<IActionResult> Chartjs()
        {
            var Assests = await _context.Assets.ToListAsync();   
            return View(Assests);
        }

        #region other view methods 
        public ActionResult Apex()
        {
            return View();
        }



        public ActionResult Echart()
        {
            int x = 1;
            return View();
        }

        public ActionResult Flot()
        {
            return View();
        }

        public ActionResult Knob()
        {
            return View();
        }

        public ActionResult Sparkline()
        {
            return View();
        }

        public ActionResult ChartTui()
        {
            return View();
        }

        #endregion


    }
}