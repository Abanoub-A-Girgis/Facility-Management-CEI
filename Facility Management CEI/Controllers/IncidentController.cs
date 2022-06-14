using API.DB;
using API.Models;
using API.Moldels_DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facility_Management_CEI.Controllers
{
    public class IncidentController : Controller
    {
        public ApplicationDBContext _Context { get; set; }
        public IncidentController(ApplicationDBContext context)
        {
            _Context = context;
        }
        // GET: IncidentController
        public ActionResult Index()//index is considered to be the home page of our controller
        {
            var Incident = _Context.Incidents.ToList();
            return View(Incident);//pass the incident table to our view to be used inside the model property 
        }

        // GET: IncidentController/Details/5
        public async Task<ActionResult> Details(int? IncidentId)
        {
            if (IncidentId==null)//make sure that the id is not null 
            {
                return NotFound();//return not found page if the id is null
            }
            var Incident = await _Context.Incidents.FirstOrDefaultAsync(m => m.Id == IncidentId); //searching for the incident that has the same id within our data base
            if (Incident == null) //if there is no incident with the following id 
            {
                return NotFound();//return not found page 
            }
            else
            {
                return View(Incident);//return the incident to be dispalyed at our page 
            }
        }

        // GET: IncidentController/Create
        public ActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> AddOrEdit(int? IncidentId)
        {
            //if the id is null then the user wants to create a new record and the page name will beCreate student
            //on the other hand if the id has a value then the user wants to update a current record 
            ViewBag.PageName = IncidentId == null ? "Create Incident" : "Edit Incident";
            ViewBag.IsEdit = IncidentId == null ? false : true;
            if (IncidentId == null)
            {
                return View();//where is the syntax of this View In cae that i want to mofdify any thing (which view will be returned)
            }
            else
            {
                var Incident = await _Context.Incidents.FindAsync(IncidentId);//search for the student by the given id 

                if (Incident == null)
                {
                    return NotFound();//throgh an exception if not found 
                }
                ViewData["SensorWarningId"] = new SelectList(_Context.SensorWarnings, "id", "id");
                ViewData["AssetId"] = new SelectList(_Context.Assets, "id", "id");
                ViewData["UserId"] = new SelectList(_Context.Users, "id", "id");
                ViewData["SpaceId"] = new SelectList(_Context.Spaces, "id", "id");
                return View(Incident);//pass this student and return the view 
            }//the returned view contains the UI cells that will allow you to insert or edit records  
        }
        // POST: IncidentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int Id, [Bind("AssetId,Description,Priority,SensorWarningId,Status,UserId,SpaceId,ReporingTime")] Incident incidentData)
        {
            bool IsIncidentExist = false;//check if this student is already exsit

            Incident Incident = await _Context.Incidents.FindAsync(Id);//search for the student by the given id

            if (Incident != null) //if exist 
            {
                IsIncidentExist = true;
            }
            else
            {
                Incident = new Incident();//if it doesn`t exist create new student object 
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //read the input data(this step is done in both cases bot the action of adding or creating is determined later )
                    Incident.Description = incidentData.Description;
                    Incident.Priority= incidentData.Priority;
                    Incident.Status= incidentData.Status;
                    ViewData["SensorWarningId"] = new SelectList(_Context.SensorWarnings,"id","id");
                    ViewData["AssetId"] = new SelectList(_Context.SensorWarnings, "id", "id");
                    ViewData["UserId"] = new SelectList(_Context.SensorWarnings, "id", "id");
                    ViewData["SpaceId"] = new SelectList(_Context.SensorWarnings, "id", "id");
                    Incident.ReportingTime= DateTime.Now;
                    if (IsIncidentExist)//if this flag is true then update the current student data 
                    {
                        _Context.Update(Incident);
                    }
                    else
                    {
                        _Context.Add(Incident);//if this flag is false then add student data
                    }
                    await _Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(incidentData);
        }
        // GET: IncidentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: IncidentController/Edit/5
        [HttpPost]
        //ValidateAntiForgeryToken attribute: is to prevent cross-site request forgery attacks. A cross-site request forgery is an attack in which a harmful script element, malicious command, or code is sent from the browser of a trusted user.
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: IncidentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: IncidentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
