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
    public class BuildingsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public BuildingsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Buildings
        [Authorize(Roles = "SystemAdmin,Manager,Owner")]
        public IActionResult BuildingComponents()
        {
            return View();
        }
        // GET: Buildings
        [Authorize(Roles = "SystemAdmin,Manager,Owner")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Buildings.ToListAsync());
        }

        // GET: Buildings/Details/5
        [Authorize(Roles = "SystemAdmin,Manager,Owner")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await _context.Buildings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

    }
}
