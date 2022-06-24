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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Facility_Management_CEI.APIs.Models;
using Xbim.Ifc;
using System.Configuration;
using Xbim.Ifc2x3.MaterialResource;

namespace Skote.Controllers
{
    public class TaskPagesController : Controller
    {
        bool IsEdited = true; //ths is made to distenguish if the element is in edit state or create state
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
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector")]
        public async Task <IActionResult> CreateTask()
        {
            IsEdited = false;
            ViewBag.IsEdited = IsEdited;
            //the next 3 lines are made to find the sign in user 
            var user = await _userManeger.GetUserAsync(User);
            var userId = user.Id;
            ViewBag.UserId = _Context.AppUsers.ToList().Where(u => u.LogUserId == userId).FirstOrDefault().Id;
            ViewData["IncidentId"] = new SelectList(_Context.Incidents, "Id", "Id");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector")]
        public async Task<IActionResult> CreateTask([Bind(" Id,AssignedById,AssignedToId,Cost,CreatedById,Description,IncidentId,FixingTime,Type,Status,Priority")] Task task)
        {

            if (ModelState.IsValid)
            {
                task.Status = API.Enums.TaskStatus.NotAssigned;
                _Context.Add(task);
                await _Context.SaveChangesAsync();
                return RedirectToAction("TaskList");
            }
            ViewData["IncidentId"] = new SelectList(_Context.Incidents, "Id", "Id");
            return View(task);
        }
   
        [HttpGet]
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector,Agent")]
        public IActionResult TaskList()
        {
            var tasks = _Context.Tasks.ToList();
            ViewBag.ListOfTasks = tasks;
            ViewBag.Users = _Context.AppUsers.ToList();
            return View();
        }
        //show details by id we may call is search
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector,Agent")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _Context.Tasks
                //.Include(t => t.AssignedBy)
                //.Include(t => t.AssignedTo)
                //.Include(t => t.CreatedBy)
                .Include(t => t.Incident)
                .ThenInclude(i => i.Space)
                .ThenInclude(s => s.Floor)
                .ThenInclude(f => f.Building)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            string FilePath = task.Incident.Space.Floor.Building.Path;
            ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../" + FilePath.Substring(0, FilePath.Length - 3) + "wexBIM");
            //List<IfcMaterial> Materials;

            //using (IfcStore Model = IfcStore.Open("wwwroot/" + FilePath))
            //{
            //    var building = Model.Instances;
            //    var asset = building.FirstOrDefault(i => i.EntityLabel == task.Incident.AssetId).Material;
            //    var test = "test";
            //}

            //ViewBag.Materials = Materials;
            return View(task);
        }
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector")]
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
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector")]
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
                    task.Status = API.Enums.TaskStatus.WorkInProgress;
                    //task.IncidentId = TaskData.IncidentId;
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
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager")]
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
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager")]
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