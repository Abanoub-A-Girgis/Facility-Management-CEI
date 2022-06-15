using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Facility_Management_CEI.IdentityDb;
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
            //passing the data from the view to the model 
            ViewBag.Buildings = await _context.Buildings.ToListAsync();
            ViewBag.Floors = await _context.Floors.ToListAsync();
            ViewBag.Spaces = await _context.Spaces.ToListAsync();
            ViewBag.Assets = await _context.Assets.ToListAsync();
            ViewBag.SensorWarnings = await _context.SensorWarnings.ToListAsync();
            ViewBag.Tasks = await _context.Tasks.ToListAsync();
            ViewBag.Users = await _context.AppUsers.ToListAsync();
            ViewBag.Incidents = await _context.Incidents.ToListAsync();
            //ViewBag.Sensors = await _context.Sensors.ToListAsync(); error occurs that unable to cast from string to int32
            return View();
        }

        #region other view methods 
        //public ActionResult Apex()
        //{
        //    return View();
        //}



        //public ActionResult Echart()
        //{
        //    int x = 1;
        //    return View();
        //}

        //public ActionResult Flot()
        //{
        //    return View();
        //}

        //public ActionResult Knob()
        //{
        //    return View();
        //}

        //public ActionResult Sparkline()
        //{
        //    return View();
        //}

        //public ActionResult ChartTui()
        //{
        //    return View();
        //}

        #endregion


    }
}