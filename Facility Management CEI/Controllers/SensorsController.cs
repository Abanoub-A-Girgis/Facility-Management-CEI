using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Authorization;

namespace Facility_Management_CEI.Controllers
{
    public class SensorsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public SensorsController(ApplicationDBContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector,Owner")]
        // GET: Sensors
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.Sensors.Include(s => s.Asset).Include(s => s.Space);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: Sensors/Details/5
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector,Owner")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensor = await _context.Sensors
                .Include(s => s.Asset)
                .Include(s => s.Space)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sensor == null)
            {
                return NotFound();
            }

            return View(sensor);
        }
    }
}
