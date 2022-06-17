﻿using Facility_Management_CEI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Xbim.Ifc;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.ModelGeometry.Scene;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Facility_Management_CEI.IdentityDb;
using Microsoft.EntityFrameworkCore;

namespace Facility_Management_CEI.Controllers
{
    public class ViewerParameter
    {
        //public int ManagerId;
        //public int SuperVisorId;
        //public int InspectorId;
        //public int AgentId;
        public List<int> Severe = new List<int>();
        public List<int> High = new List<int>();
        public List<int> Medium = new List<int>();
        public List<int> Low = new List<int>();
    }

    //public class testView
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
    
    public class ViewerController : Controller
    {
        public ApplicationDBContext _context { get; set; }

        private readonly ILogger<HomeController> _logger;

        public ViewerController(ILogger<HomeController> logger, ApplicationDBContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult ViewerTest()
        {
            return View();
        }
        
        public ViewerParameter fillViewParameterForAgents(List<API.Models.Task> Tasks, ViewerParameter viewerParam = null)
        {
            if (viewerParam == null)
            {
                viewerParam = new ViewerParameter();
            }
            
            foreach (var t in Tasks)
            {
                if (t.Priority == API.Enums.Priority.Severe)
                {
                    viewerParam.Severe.Add((int)t.Incident.AssetId);
                }
                else if (t.Priority == API.Enums.Priority.High)
                {
                    viewerParam.High.Add((int)t.Incident.AssetId);
                }
                else if (t.Priority == API.Enums.Priority.Medium)
                {
                    viewerParam.Medium.Add((int)t.Incident.AssetId);
                }
                else if (t.Priority == API.Enums.Priority.Low)
                {
                    viewerParam.Low.Add((int)t.Incident.AssetId);
                }
            }
            return viewerParam;
        }

        public async Task<IActionResult> ViewerAsAgent(int EmployeeId)
        {
            var Tasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed).Include(t => t.Incident).ThenInclude(i => i.Asset).ToListAsync();
            ViewerParameter viewerParam = fillViewParameterForAgents(Tasks);
            ViewBag.Tasks = Tasks;
            //List<testView> test1 = new List<testView>(){ new testView { Id = 1, Name = "Potato" } };
            //List<testView> test2 = new List<testView>() { new testView { Id = 2, Name = "orange"},
            //                       new testView{ Id = 2, Name = "orange", }
            //                    };
            //ViewBag.test1 = JsonSerializer.Serialize(test1);
            //ViewBag.test2 = JsonSerializer.Serialize(test2);
            return View(viewerParam);
        }

        public async Task<IActionResult> ViewerAsInspector(int InspectorId)
        {
            var Agents = await _context.AppUsers.Where(u => u.SuperId == InspectorId).ToListAsync();
            ViewerParameter viewerParam = new ViewerParameter();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var a in Agents)
            {
                int EmployeeId = a.Id;
                var AgentTasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed).Include(t => t.Incident).ThenInclude(i => i.Asset).ToListAsync();
                Tasks.AddRange(AgentTasks);
                viewerParam= fillViewParameterForAgents(AgentTasks, viewerParam);
            }
            ViewBag.Tasks = Tasks;
            ViewBag.Agents = Agents; 
            return View(viewerParam);
        }
        
        public async Task<IActionResult> ViewerAsSupervisor(int SupervisorId)
        {
            List<API.Models.AppUser> Agents = new List<API.Models.AppUser>();
            var Inspectors = await _context.AppUsers.Where(u => u.SuperId == SupervisorId).ToListAsync();
            ViewerParameter viewerParam = new ViewerParameter();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var i in Inspectors)
            {
                var InspectorAgents = await _context.AppUsers.Where(u => u.SuperId == i.Id).ToListAsync();
                Agents.AddRange(InspectorAgents);
                foreach (var a in InspectorAgents)
                {
                    int EmployeeId = a.Id;
                    var AgentTasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed).Include(t => t.Incident).ThenInclude(i => i.Asset).ToListAsync();
                    Tasks.AddRange(AgentTasks);
                    viewerParam = fillViewParameterForAgents(AgentTasks, viewerParam);
                }
            }
            ViewBag.Tasks = Tasks;
            ViewBag.Agents = Agents;
            return View(viewerParam);
        }

        public async Task<IActionResult> ViewerAsManager(int ManagerId)
        {
            List<API.Models.AppUser> Agents = new List<API.Models.AppUser>();
            var Supervisors = _context.AppUsers.Where(u => u.SuperId == ManagerId).ToList();
            ViewerParameter viewerParam = new ViewerParameter();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var supervisor in Supervisors)
            {
                var Inspectors = await _context.AppUsers.Where(u => u.SuperId == supervisor.Id).ToListAsync();
                foreach (var i in Inspectors)
                {
                    var InspectorAgents = await _context.AppUsers.Where(u => u.SuperId == i.Id).ToListAsync();
                    Agents.AddRange(InspectorAgents);
                    foreach (var a in InspectorAgents)
                    {
                        int EmployeeId = a.Id;
                        var AgentTasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed).Include(t => t.Incident).ThenInclude(i => i.Asset).ToListAsync();
                        Tasks.AddRange(AgentTasks);
                        viewerParam = fillViewParameterForAgents(AgentTasks, viewerParam);
                    }
                }
            }
            ViewBag.Tasks = Tasks;
            ViewBag.Agents = Agents; 
            return View(viewerParam);
        }

        public async Task<IActionResult> ViewerAsOwner(int OwnerId)
        {
            List<API.Models.AppUser> Agents = new List<API.Models.AppUser>();
            var Mangagers = _context.AppUsers.Where(u => u.SuperId == OwnerId).ToList();
            ViewerParameter viewerParam = new ViewerParameter();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var Manager in Mangagers)
            {
                var Supervisors = _context.AppUsers.Where(u => u.SuperId == Manager.Id).ToList();
                foreach (var supervisor in Supervisors)
                {
                    var Inspectors = await _context.AppUsers.Where(u => u.SuperId == supervisor.Id).ToListAsync();
                    foreach (var i in Inspectors)
                    {
                        var InspectorAgents = await _context.AppUsers.Where(u => u.SuperId == i.Id).ToListAsync();
                        Agents.AddRange(InspectorAgents);
                        foreach (var a in InspectorAgents)
                        {
                            int EmployeeId = a.Id;
                            var AgentTasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed).Include(t => t.Incident).ThenInclude(i => i.Asset).ToListAsync();
                            Tasks.AddRange(AgentTasks);
                            viewerParam = fillViewParameterForAgents(AgentTasks, viewerParam);
                        }
                    }
                }
            }
            ViewBag.Agents = Agents;
            ViewBag.Tasks = Tasks;
            return View(viewerParam);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
