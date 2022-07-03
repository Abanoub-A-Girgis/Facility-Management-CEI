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
    public class FloorsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public FloorsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Floors
        [Authorize(Roles = "AccountManager,Manager,Owner")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var applicationDBContext = _context.Floors.Include(f => f.Building);
                return View(await applicationDBContext.ToListAsync());
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }

        }

        // GET: Floors/Details/5
        [Authorize(Roles = "AccountManager,Manager,Owner")]
        public async Task<IActionResult> Details(int? id)
        {
            try
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
            catch (Exception ex)
            {
                TempData["message"] = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }
           
        }

    }
}
