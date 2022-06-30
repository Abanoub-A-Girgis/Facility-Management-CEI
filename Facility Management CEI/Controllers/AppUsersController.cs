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
    public class AppUsersController : Controller
    {
        private readonly ApplicationDBContext _context;

        public AppUsersController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: AppUsers
        public async Task<IActionResult> Index()
        {
            try
            {
                var applicationDBContext = _context.AppUsers.Include(a => a.Building).Include(a => a.LogUser).Include(a => a.Super);
                return View(await applicationDBContext.ToListAsync());
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }
           
        }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var appUser = await _context.AppUsers
                    .Include(a => a.Building)
                    .Include(a => a.LogUser)
                    .Include(a => a.Super)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (appUser == null)
                {
                    return NotFound();
                }

                return View(appUser);
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }

        }

        // GET: AppUsers/Create
        public IActionResult Create()
        {
            try
            {
                ViewData["BuildingId"] = new SelectList(_context.Buildings, "Id", "Id");
                ViewData["Id"] = new SelectList(_context.AppUsers, "Id", "Id");
                ViewData["SuperId"] = new SelectList(_context.AppUsers, "Id", "Id");
                return View();
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }
            
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Type,BuildingId,SuperId,LogUserId")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuildingId"] = new SelectList(_context.Buildings, "Id", "Id", appUser.BuildingId);
            ViewData["Id"] = new SelectList(_context.AppUsers, "Id", "Id",appUser.Id);
            ViewData["SuperId"] = new SelectList(_context.AppUsers, "Id", "Id", appUser.SuperId);
            return View(appUser);
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var appUser = await _context.AppUsers.FindAsync(id);
                if (appUser == null)
                {
                    return NotFound();
                }
                ViewData["BuildingId"] = new SelectList(_context.Buildings, "Id", "Id", appUser.BuildingId);
                ViewData["Id"] = new SelectList(_context.AppUsers, "Id", "Id", appUser.Id);
                ViewData["SuperId"] = new SelectList(_context.AppUsers, "Id", "Id", appUser.SuperId);
                return View(appUser);
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }
            
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Type,BuildingId,SuperId,LogUserId")] AppUser appUser)
        {
            try
            {
                if (id != appUser.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(appUser);
                        var LogUser = await _context.LogUsers.FirstOrDefaultAsync(u => u.Id == appUser.LogUserId);
                        if (LogUser != null)
                        {
                            LogUser.FirstName = appUser.FirstName;
                            LogUser.LastName = appUser.LastName;
                            _context.Update(LogUser);
                        }
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AppUserExists(appUser.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                ViewData["BuildingId"] = new SelectList(_context.Buildings, "Id", "Id", appUser.BuildingId);
                ViewData["Id"] = new SelectList(_context.AppUsers, "Id", "Id", appUser.Id);
                ViewData["SuperId"] = new SelectList(_context.AppUsers, "Id", "Id", appUser.SuperId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }
           
        }

        // GET: AppUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var appUser = await _context.AppUsers
                    .Include(a => a.Building)
                    .Include(a => a.LogUser)
                    .Include(a => a.Super)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (appUser == null)
                {
                    return NotFound();
                }

                return View(appUser);
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }
           
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var appUser = await _context.AppUsers
                    .Include(i => i.Incidents)
                    .Include(i => i.SensorWarnings)
                    .Include(i => i.TasksCreated)
                    .Include(i => i.TasksAssigned)
                    .Include(i => i.TasksReceived)
                    .Include(i => i.SuperUnder).FirstOrDefaultAsync(m => m.Id == id);
                var LogUser = appUser.LogUser;
                foreach (var user in appUser.SuperUnder)
                {
                    user.SuperId = null;
                }
                foreach (var task in appUser.TasksReceived)
                {
                    task.AssignedToId = null;
                }
                foreach (var task in appUser.TasksAssigned)
                {
                    task.AssignedById = null;
                }
                foreach (var SensorWarning in appUser.SensorWarnings)
                {
                    SensorWarning.AppUserId = null;
                }
                _context.Tasks.RemoveRange(appUser.TasksCreated);
                _context.Incidents.RemoveRange(appUser.Incidents);
                _context.AppUsers.Remove(appUser);
                if (LogUser != null)
                {
                    _context.LogUsers.Remove(LogUser);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message.ToString();
                return RedirectToAction("ErrorGeneric", "ErrorPages");
            }

        }
        private bool AppUserExists(int id)
        {
            return _context.AppUsers.Any(e => e.Id == id);
        }
    }
}
