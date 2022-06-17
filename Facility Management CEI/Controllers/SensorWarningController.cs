using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using API.Models;
using API.Enums;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Facility_Management_CEI.Controllers
{
    public class SensorWarningController : Controller
    {
        public ApplicationDBContext _context { get; set; }
        public SensorWarningController(ApplicationDBContext context)
        {
            _context = context;
        }


        //[Authorize(Roles ="Admin,Manger,Supervisor,Inspector")]
        public async Task<IActionResult> Index(int? warningId)
        {
            if (warningId==null)
            {
                int TrialsNumber = 3;
                var Sensor = await _context.Sensors.ToListAsync();
                int count = Sensor.Count;
                var priority = Priority.Medium;
                Random MyRandom = new Random();
                Random random = new Random();
                List<SensorWarning> WariningList = new List<SensorWarning>();
                for (int i = 0; i < TrialsNumber; i++)
                {
                    var SensorId = MyRandom.Next(0, count);
                    var MySensor = Sensor.ElementAt(SensorId);
                    var spaceId = MySensor.SpaceId;
                    var MySpace = _context.Spaces.ToList().Where(e => e.Id == spaceId).FirstOrDefault();
                    var SpaceName = MySpace.Name;
                    var FloorId = MySpace.FloorId;
                    var MyFloor = _context.Floors.ToList().Where(e => e.Id == FloorId).FirstOrDefault();
                    var FloorName = MyFloor.FloorName;
                    var temp = random.Next(0, 100);
                    var Smoke = random.Next(0, 100);
                    var people = random.Next(0, 100);
                    //this switch statment Priority Values
                    switch (MySensor.SensorType)
                    {
                        case SensorType.SpaceSensor:

                            if (MySensor.Name.Contains("Temperature"))
                            {
                                if (temp <= 25)
                                {
                                    priority = Priority.Low;
                                }
                                if (temp > 25 && temp <= 50)
                                {
                                    priority = Priority.Medium;
                                }
                                if (temp > 50 && temp <= 75)
                                {
                                    priority = Priority.High;
                                }
                                if (temp > 75 && temp <= 100)
                                {
                                    priority = Priority.Severe;
                                }
                            }
                            if (MySensor.Name.Contains("Smoke"))
                            {
                                if (Smoke <= 25)
                                {
                                    priority = Priority.Low;
                                }
                                if (Smoke > 25 && Smoke <= 50)
                                {
                                    priority = Priority.Medium;
                                }
                                if (Smoke > 50 && Smoke <= 75)
                                {
                                    priority = Priority.High;
                                }
                                if (Smoke > 75 && Smoke <= 100)
                                {
                                    priority = Priority.Severe;
                                }
                            }
                            if (MySensor.Name.Contains("Health"))
                            {
                                if (people <= 25)
                                {
                                    priority = Priority.Low;
                                }
                                if (people > 25 && people <= 50)
                                {
                                    priority = Priority.Medium;
                                }
                                if (people > 50 && people <= 75)
                                {
                                    priority = Priority.High;
                                }
                                if (people > 75 && people <= 100)
                                {
                                    priority = Priority.Severe;
                                }
                            }
                            break;
                        case SensorType.AssetSensor:
                            priority = Priority.Severe;
                            break;
                        default:
                            break;
                    }
                    WariningList.Add
                    (
                      new SensorWarning()
                    {
                        IssueDate = DateTime.Now,
                        Description = $"there is a problem  at Storey: {MyFloor.FloorName} Witnin Space: {MySensor.Space.Name} -Id ({MySensor.Space.Id})",
                        Investigated = false,
                        Priority = priority,
                        InvestigatDate = null,
                        SensorId = MySensor.Id,
                        AppUserId = null
                    });
                }
                _context.AddRange(WariningList);
                _context.SaveChanges();
                var Mylist = _context.SensorWarnings.ToList(); 
                return View(Mylist);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Id,IssueDate,Description,Priority,Investigated,Status,AppUserId,SensorId,InvestigatDate")] SensorWarning WarningData) 
        {
            SensorWarning Warning = await _context.SensorWarnings.FindAsync(WarningData.Id);
            if (ModelState.IsValid)
            {
                try
                {
                    Warning.Description=WarningData.Description;
                    Warning.Investigated=WarningData.Investigated;
                    Warning.InvestigatDate=WarningData.InvestigatDate;
                    Warning.IssueDate=WarningData.IssueDate; 
                    Warning.AppUserId=WarningData.AppUserId;
                    Warning.Priority=WarningData.Priority;
                    Warning.SensorId=WarningData.SensorId;
                    _context.Update(Warning);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            var Mylist = _context.SensorWarnings.ToList();
            return NoContent();
        }
        public async Task<IActionResult> Ignore(int? WarningId)
        {
            if (WarningId == null)
            {
                return NotFound();
            }
            var Incident = await _context.SensorWarnings.FirstOrDefaultAsync(m => m.Id == WarningId);

            return View(Incident);
        }
        public async Task<ActionResult> Investigate(int? WarningId)
        {
            if (WarningId == null)//make sure that the id is not null 
            {
                return NotFound();//return not found page if the id is null
            }
            var SensorWarning = await _context.SensorWarnings.FirstOrDefaultAsync(m => m.Id == WarningId); //searching for the incident that has the same id within our data base
            if (SensorWarning == null) //if there is no incident with the following id 
            {
                return NotFound();//return not found page 
            }
            else
            {
                return View(SensorWarning);//return the incident to be dispalyed at our page 
            }
        }
    }
}
