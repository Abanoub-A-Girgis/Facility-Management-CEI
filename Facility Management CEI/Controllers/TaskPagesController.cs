using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Moldels_DTOs;
using Microsoft.AspNetCore.Mvc;
using Facility_Management_CEI.IdentityDb;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Task = API.Models.Task;
using Microsoft.AspNetCore.Mvc.Rendering;
using API.Enums;
using Microsoft.AspNetCore.Identity;
using Facility_Management_CEI.APIs.Models;

namespace Skote.Controllers
{
    public class TaskPagesController : Controller
    {
        private readonly UserManager<LogUser> _userManeger;
        private readonly SignInManager<LogUser> _signInManager;
        public ApplicationDBContext _Context { get; set; }
        public TaskPagesController(UserManager<LogUser> userManger, ApplicationDBContext context, SignInManager<LogUser> singInManager)
        {
            this._userManeger = userManger;
            this._Context = context;
            this._signInManager = singInManager;
        }

        //Create ur task
        public async Task <IActionResult> CreateTask()
        {
            //ViewData["AssignedById"] = new SelectList(_Context.AppUsers, "Id", "Id");
            //ViewData["CreatedById"] = new SelectList(_Context.AppUsers, "Id", "Id");
            //ViewData["AssignedToId"] = new SelectList(_Context.AppUsers, "Id", "Id");
            var user = await _userManeger.GetUserAsync(User);
            var userId = user.Id;
            ViewBag.UserId = _Context.AppUsers.ToList().Where(u => u.LogUserId == userId).FirstOrDefault().Id;
            ViewData["IncidentId"] = new SelectList(_Context.Incidents, "Id", "Id");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask([Bind(" Id,AssignedById,AssignedToId,Cost,CreatedById,Description,IncidentId,FixingTime,Type,Status,Priority")] Task task)
        {

            if (ModelState.IsValid)
            {
                _Context.Add(task);
                await _Context.SaveChangesAsync();
                return RedirectToAction("TaskList");
            }

            //ViewData["AssignedById"] = new SelectList(_Context.AppUsers, "Id", "Id");
            //ViewData["AssignedToId"] = new SelectList(_Context.AppUsers, "Id", "Id");
            //ViewData["CreatedById"] = new SelectList(_Context.AppUsers, "Id", "Id");
            ViewData["IncidentId"] = new SelectList(_Context.Incidents, "Id", "Id");
            return View(task);
        }
   
        [HttpGet]
        public IActionResult TaskList()
        {
            var tasks = _Context.Tasks.ToList();
            ViewBag.ListOfTasks = tasks;
            return View();
        }
        //show details by id we may call is search
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _Context.Tasks
                .Include(t => t.AssignedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Incident)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _Context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var user = await _userManeger.GetUserAsync(User);
            var userId = user.Id;
            ViewBag.UserId = _Context.AppUsers.ToList().Where(u => u.LogUserId == userId).FirstOrDefault().Id;
            ViewData["AssignedById"] = new SelectList(_Context.AppUsers, "Id", "Id", task.AssignedById);
            ViewData["AssignedToId"] = new SelectList(_Context.AppUsers, "Id", "Id", task.AssignedToId);
            ViewData["IncidentId"] = new SelectList(_Context.Incidents, "Id", "Id", task.IncidentId);
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Description,Status,Priority,Cost,FixingTime,IncidentId,CreatedById,AssignedToId,AssignedById,Comment")] Task task)
        {

            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _Context.Update(task);
                    await _Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(TaskList));
            }
            //ViewData["CreatedById"] = new SelectList(_Context.AppUsers, "Id", "LogUserId", task.CreatedById);
            ViewData["AssignedById"] = new SelectList(_Context.AppUsers, "Id", "Id", task.AssignedById);
            ViewData["AssignedToId"] = new SelectList(_Context.AppUsers, "Id", "Id", task.AssignedToId);
            ViewData["IncidentId"] = new SelectList(_Context.Incidents, "Id", "Id", task.IncidentId);
            return View(task);
        }
        public async Task<IActionResult> Assign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _Context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var user = await _userManeger.GetUserAsync(User);
            var userId = user.Id;
            ViewBag.UserId = _Context.AppUsers.ToList().Where(u => u.LogUserId == userId).FirstOrDefault().Id;
            //ViewData["AssignedById"] = new SelectList(_Context.AppUsers, "Id", "Id", task.AssignedById);
            ViewData["AssignedToId"] = new SelectList(_Context.AppUsers, "Id", "Id", task.AssignedToId);
            ViewData["IncidentId"] = new SelectList(_Context.Incidents, "Id", "Id", task.IncidentId);
            return View(task);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, [Bind("Id,IncidentId,CreatedById,AssignedToId,AssignedById")] Task TaskData)
        {
            var task = await _Context.Tasks.FindAsync(id);
            if (id != TaskData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    task.IncidentId = TaskData.IncidentId;
                    task.CreatedById=TaskData.CreatedById;
                    task.AssignedToId=TaskData.AssignedToId;
                    task.AssignedById=TaskData.AssignedById;
                    _Context.Update(task);
                    await _Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(TaskData.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(TaskList));
            }
            //ViewData["CreatedById"] = new SelectList(_Context.AppUsers, "Id", "LogUserId", task.CreatedById);
            //ViewData["AssignedById"] = new SelectList(_Context.AppUsers, "Id", "Id", task.AssignedById);
            //ViewData["AssignedToId"] = new SelectList(_Context.AppUsers, "Id", "Id", TaskData.AssignedToId);
            //ViewData["IncidentId"] = new SelectList(_Context.Incidents, "Id", "Id", TaskData.IncidentId);
            return View(TaskData);
        }
        //delete func..it's important to create this before using any http verb as it works like get and then we use post verb in the next function 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _Context.Tasks
                .Include(t => t.AssignedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Incident)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _Context.Tasks.FindAsync(id);
            _Context.Tasks.Remove(task);
            await _Context.SaveChangesAsync();
            return RedirectToAction(nameof(TaskList));
        }




        private bool TaskExists(int id)
        {
            return _Context.Tasks.Any(e => e.Id == id);
        }
    }
}