using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Moldels_DTOs;
using Microsoft.AspNetCore.Mvc;
using API.DB;
using Microsoft.EntityFrameworkCore;
using API.Models;
using Task = API.Models.Task;
using Microsoft.AspNetCore.Mvc.Rendering;
using API.Enums;

namespace Skote.Controllers
{
    public class TaskPagesController : Controller
    {
        public ApplicationDBContext _Context { get; set; }
        public TaskPagesController(ApplicationDBContext context)
        {
            _Context = context;
        }

        //Create ur task
        public IActionResult CreateTask()
        {
            ViewData["AssignedById"] = new SelectList(_Context.Users, "Id", "Id");
            ViewData["AssignedToId"] = new SelectList(_Context.Users, "Id", "Id");
            ViewData["CreatedById"] = new SelectList(_Context.Users, "Id", "Id");
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

            ViewData["AssignedById"] = new SelectList(_Context.Users, "Id", "Id");
            ViewData["AssignedToId"] = new SelectList(_Context.Users, "Id", "Id");
            ViewData["CreatedById"] = new SelectList(_Context.Users, "Id", "Id");
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

    }
}