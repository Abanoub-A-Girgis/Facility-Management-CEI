﻿using System;
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
    public class SpacesController : Controller
    {
        private readonly ApplicationDBContext _context;

        public SpacesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Spaces
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.Spaces.Include(s => s.Floor);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: Spaces/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var space = await _context.Spaces
                .Include(s => s.Floor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (space == null)
            {
                return NotFound();
            }

            return View(space);
        }

    }
}