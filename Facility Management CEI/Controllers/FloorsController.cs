using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Facility_Management_CEI.IdentityDb;

namespace Facility_Management_CEI.Controllers
{
    public class FloorsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public FloorsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Floors
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.Floors.Include(f => f.Building);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: Floors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var floor = await _context.Floors
                .Include(f => f.Building)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (floor == null)
            {
                return NotFound();
            }

            return View(floor);
        }

    }
}
