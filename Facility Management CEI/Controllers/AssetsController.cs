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
    public class AssetsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public AssetsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Assets
        public async Task<IActionResult> Index()
        {
            try
            {
                var applicationDBContext = _context.Assets.Include(a => a.Floor).Include(a => a.Space);
                return View(await applicationDBContext.ToListAsync());
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("Error404", "ErrorPages");
            }

        }

        // GET: Assets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var asset = await _context.Assets
                    .Include(a => a.Floor)
                    .Include(a => a.Space)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (asset == null)
                {
                    return NotFound();
                }

                return View(asset);
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("Error404", "ErrorPages");
            }

        }

    }
}
