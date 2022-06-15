using Facility_Management_CEI.Models;
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
using API.DB;
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
        
        public ViewerParameter fillViewParameterForAgents(List<API.Models.Task> Tasks)
        {
            ViewerParameter viewerParam = new ViewerParameter();
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
            return View(viewerParam);
        }

        public async Task<IActionResult> ViewerAsInspector(int InspectorId)
        {
            var Agents = await _context.AppUsers.Where(u => u.SuperId == InspectorId).ToListAsync();
            Dictionary<int, ViewerParameter> viewerParam = new Dictionary<int, ViewerParameter>();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var a in Agents)
            {
                int EmployeeId = a.Id;
                var AgentTasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed).Include(t => t.Incident).ThenInclude(i => i.Asset).ToListAsync();
                Tasks.AddRange(AgentTasks);
                viewerParam[EmployeeId] = fillViewParameterForAgents(AgentTasks);
            }
            ViewBag.Tasks = Tasks;
            return View(viewerParam);
        }

        [HttpPost]
        public async Task<IActionResult> ViewerAsSupervisor(int SupervisorId)
        {
            var Inspectors = await _context.AppUsers.Where(u => u.SuperId == SupervisorId).ToListAsync();
            Dictionary<int, ViewerParameter> viewerParam = new Dictionary<int, ViewerParameter>();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var i in Inspectors)
            {
                var Agents = await _context.AppUsers.Where(u => u.SuperId == i.Id).ToListAsync();
                foreach (var a in Agents)
                {
                    int EmployeeId = a.Id;
                    var AgentTasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed).Include(t => t.Incident).ThenInclude(i => i.Asset).ToListAsync();
                    Tasks.AddRange(AgentTasks);
                    viewerParam[EmployeeId] = fillViewParameterForAgents(AgentTasks);
                }
            }
            ViewBag.Tasks = Tasks;
            return View(viewerParam);
        }

        public async Task<IActionResult> ViewerAsManager(int ManagerId)
        {
            var Supervisors = _context.AppUsers.Where(u => u.SuperId == ManagerId).ToList();
            Dictionary<int, ViewerParameter> viewerParam = new Dictionary<int, ViewerParameter>();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var supervisor in Supervisors)
            {
                var Inspectors = await _context.AppUsers.Where(u => u.SuperId == supervisor.Id).ToListAsync();
                foreach (var i in Inspectors)
                {
                    var Agents = await _context.AppUsers.Where(u => u.SuperId == i.Id).ToListAsync();
                    foreach (var a in Agents)
                    {
                        int EmployeeId = a.Id;
                        var AgentTasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed).Include(t => t.Incident).ThenInclude(i => i.Asset).ToListAsync();
                        Tasks.AddRange(AgentTasks);
                        viewerParam[EmployeeId] = fillViewParameterForAgents(AgentTasks);
                    }
                }
            }
            ViewBag.Tasks = Tasks;
            return View(viewerParam);
        }

        public async Task<IActionResult> ViewerAsOwner(int OwnerId)
        {
            var Mangagers = _context.AppUsers.Where(u => u.SuperId == OwnerId).ToList();
            Dictionary<int, ViewerParameter> viewerParam = new Dictionary<int, ViewerParameter>();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var Manager in Mangagers)
            {
                var Supervisors = _context.AppUsers.Where(u => u.SuperId == Manager.Id).ToList();
                foreach (var supervisor in Supervisors)
                {
                    var Inspectors = await _context.AppUsers.Where(u => u.SuperId == supervisor.Id).ToListAsync();
                    foreach (var i in Inspectors)
                    {
                        var Agents = await _context.AppUsers.Where(u => u.SuperId == i.Id).ToListAsync();
                        foreach (var a in Agents)
                        {
                            int EmployeeId = a.Id;
                            var AgentTasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed).Include(t => t.Incident).ThenInclude(i => i.Asset).ToListAsync();
                            Tasks.AddRange(AgentTasks);
                            viewerParam[EmployeeId] = fillViewParameterForAgents(AgentTasks);
                        }
                    }
                }
            }
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
