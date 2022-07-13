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
        [Authorize(Roles = "AccountManager,Owner")]
        public IActionResult BuildingComponents()
        {
            return View();
        }
        // GET: Buildings
        [Authorize(Roles = "AccountManager,Owner")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _context.Buildings.ToListAsync());

            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }
        }

        // GET: Buildings/Details/5
        [Authorize(Roles = "AccountManager,Owner")]
        public async Task<IActionResult> Details(int? id)
        {
            try
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
            catch (Exception ex)
            {
                TempData["message"] = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }

        }

    }
}
