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
        [HttpPost]
        public ActionResult CreateTask([Bind(" Id,AssignedById,AssignedToId,Cost,CreatedById,Description,IncidentId,FixingTime,Type,Status,Priority")] Task task)
        {
            //var task = new API.Models.Task();
            //task.Id= taskDTO.Id;
            //task.CreatedById = taskDTO.CreatedById;
            //task.AssignedById = taskDTO.CreatedById;
            //task.AssignedToId = taskDTO.AssignedToId;
            //task.Cost = taskDTO.Cost;
            try
            {
                if (ModelState.IsValid)
                {
                    _Context.Add(task);
                    _Context.SaveChanges();
                    return RedirectToAction("TaskList");
                }

                //API.Models.Task task = new API.Models.Task()
                //{
                //    //Id = taskDTO.Id,
                //    AssignedById = taskDTO.AssignedById,
                //    AssignedToId = taskDTO.AssignedToId,
                //    Cost = taskDTO.Cost,
                //    CreatedById = taskDTO.CreatedById,
                //    Description = taskDTO.Description,
                //    IncidentId = taskDTO.IncidentId,
                //    FixingTime = taskDTO.FixingTime,
                //    Type = taskDTO.Type,
                //    Status = taskDTO.Status,
                //    Priority = taskDTO.Priority,
                //};

                //_Context.Tasks.Add(task);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        
            return View(task);
        }
        
    //    @section Scripts // put this in the view to confirm awaiting till the data reach here again
    //    {
    //@{ await Html.RenderPartialAsync("_ValidationScriptsPartial"); }
    //    }
        //[HttpGet]
        //public IActionResult TaskShow(int id)
        //{
        //    var Task_returned = _Context.Tasks.FirstOrDefault(x => x.Id == id);
        //    ViewBag.TaskReturn = Task_returned;
        //    return View();
        //}
        //[HttpGet]
        public IActionResult TaskList()
        {
            var tasks = _Context.Tasks.ToList();
            ViewBag.ListOfTasks = tasks;
            return View();
        }

    }
}