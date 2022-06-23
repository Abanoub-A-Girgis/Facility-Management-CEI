using Facility_Management_CEI.IdentityDb;
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
using Microsoft.AspNetCore.Identity;
using Facility_Management_CEI.APIs.Models;
using Microsoft.AspNetCore.Authorization;

namespace Facility_Management_CEI.Controllers
{
    public class IncidentController : Controller
    {
        private readonly UserManager<LogUser> _userManeger;
        private readonly SignInManager<LogUser> _signInManager;
        public ApplicationDBContext _Context { get; set; }
        public IncidentController(UserManager<LogUser> userManger, ApplicationDBContext context, SignInManager<LogUser> singInManager)
        {
            this._userManeger = userManger;
            this._Context = context;
            this._signInManager = singInManager;
        }
        // GET: IncidentController
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector")]

        public ActionResult Index()//index is considered to be the home page of our controller
        {
            var Incident = _Context.Incidents.ToList();
            return View(Incident);//pass the incident table to our view to be used inside the model property 
        }

        // GET: IncidentController/Details/5
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Agent,Inspector")]

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
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector")]

        public ActionResult Create()
        {
            return View();
        }
   
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector")]
        public async Task<IActionResult> EditOrAdd(int? IncidentId)
        {
            //if the id is null then the user wants to create a new record and the page name will beCreate student
            //on the other hand if the id has a value then the user wants to update a current record 
            ViewBag.PageName = IncidentId == null ? "Create Incident" : "Edit Incident";
            ViewBag.IsEdit = IncidentId == null ? false : true;
            //these are the values from the DB to be loaded at the page openeing 
<<<<<<< HEAD
            ViewData["SensorWarningId"] = new SelectList(_Context.SensorWarnings,"Id","Id");
=======
            ViewData["SensorWarningId"] = new SelectList(_Context.SensorWarnings, "Id", "Id");
>>>>>>> c3a9aca1db12701adaea9276f22f0a74fc486830
            ViewData["AssetId"] = new SelectList(_Context.Assets, "Id", "Id");
            ViewData["SpaceId"] = new SelectList(_Context.Spaces, "Id", "Id");
            // this code is used to get the id of the current logged in user 
            var user = await _userManeger.GetUserAsync(User);
            var userId = user.Id;
            ViewBag.UserId = _Context.AppUsers.ToList().Where(u => u.LogUserId == userId).FirstOrDefault().Id;
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
                return View(Incident);//pass this Incident data to the view 
            }//the returned view contains the UI cells that will allow you to insert or edit records  
        }
        // POST: IncidentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager,Inspector")]

        //Bind will pass the data from the View of these parameters to this method in the Incident data parameters during run time, in this case we can take the run time values and place it in our database
        //for example the View will send the box that holds the value of AssetId to this method by using the binding attribute, make sure that every attribute within the biding is writtin correctly as the mapping between the run time values and back end values will differ 
        public async Task<IActionResult> EditOrAdd(int Id,[Bind("AssetId,Description,Priority,SensorWarningId,Status,AppUserId,SpaceId,ReportingTime,Comment")] Incident incidentData)
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
                    if (IsIncidentExist)//if this flag is true then update the current student data 
                    {
                        Incident.Description = incidentData.Description;
                        Incident.Priority = incidentData.Priority;
                        Incident.Status = incidentData.Status;
                        Incident.SpaceId = incidentData.SpaceId;
                        Incident.SensorWarningId = incidentData.SensorWarningId;
                        Incident.AppUserId = incidentData.AppUserId;
                        Incident.AssetId = incidentData.AssetId;
                        //Incident.ReportingTime = Incident.ReportingTime;
                        Incident.Comment= incidentData.Comment;
                        _Context.Update(Incident);
                    }
                    else
                    {
                        //read the input data(this step is done in both cases bot the action of adding or creating is determined later )
                        Incident.Description = incidentData.Description;
                        Incident.Priority = incidentData.Priority;
                        Incident.SpaceId = incidentData.SpaceId;
                        Incident.SensorWarningId = incidentData.SensorWarningId;
                        Incident.AppUserId = incidentData.AppUserId;
                        Incident.AssetId = incidentData.AssetId;
                        Incident.Comment =null;//at the creation a comment will not be writtin - the comment is used to close the incident 
                        Incident.Status = API.Enums.IncidentStatus.Open;
                        Incident.ReportingTime = DateTime.Now;
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

        [Authorize(Roles = "SystemAdmin,Supervisor,Manager")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: IncidentController/Edit/5
        [HttpPost]
        //ValidateAntiForgeryToken attribute: is to prevent cross-site request forgery attacks. A cross-site request forgery is an attack in which a harmful script element, malicious command, or code is sent from the browser of a trusted user.
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager")]
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
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager")]
        public async Task<IActionResult> Delete(int? Incidentid)
        {
            if (Incidentid == null)
            {
                return NotFound();
            }
            var Incident = await _Context.Incidents.FirstOrDefaultAsync(m => m.Id == Incidentid);

            return View(Incident);
        }

        // POST: IncidentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SystemAdmin,Supervisor,Manager")]
        public async Task<IActionResult> Delete(int Incidentid)
        {
            try
            {
                var Incident = await _Context.Incidents.FindAsync(Incidentid);
                _Context.Incidents.Remove(Incident);
                await _Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }

}
