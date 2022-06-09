﻿using API.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Facility_Management_CEI.Controllers
{
    public class SensorWarningController : Controller
    {
        public ApplicationDBContext _context { get; set; }
        public SensorWarningController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var sensorsarning = _context.SensorWarnings.Include(a => a.Sensor).ToList();
                
            if (sensorsarning == null)
            {
                return NotFound();
            }
            else
            {
                return View(sensorsarning);

            }

            //return View();
        }
    }
}
