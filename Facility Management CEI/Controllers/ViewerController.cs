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
using Facility_Management_CEI.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Facility_Management_CEI.APIs.Models;
using Microsoft.AspNetCore.Identity;

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
        private readonly UserManager<LogUser> _userManager;
        public ApplicationDBContext _context { get; set; }

        public ViewerController(UserManager<LogUser> userManager, ApplicationDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        
        public ViewerParameter fillViewParameterForAgents(List<API.Models.Task> Tasks)
        {
            //if (viewerParam == null)
            //{
            //    viewerParam = new ViewerParameter();
            //}
            
            ViewerParameter viewerParam = new ViewerParameter();
            
            foreach (var t in Tasks)
            {
                if (t.Priority == API.Enums.Priority.Severe)
                {
                    if(t.Incident.AssetId != null)
                        viewerParam.Severe.Add((int)t.Incident.AssetId);
                }
                else if (t.Priority == API.Enums.Priority.High)
                {
                    if (t.Incident.AssetId != null)
                        viewerParam.High.Add((int)t.Incident.AssetId);
                }
                else if (t.Priority == API.Enums.Priority.Medium)
                {
                    if (t.Incident.AssetId != null)
                        viewerParam.Medium.Add((int)t.Incident.AssetId);
                }
                else if (t.Priority == API.Enums.Priority.Low)
                {
                    if (t.Incident.AssetId != null)
                        viewerParam.Low.Add((int)t.Incident.AssetId);
                }
            }
            return viewerParam;
        }

        public IActionResult ViewerError()
        {
            return View();
        }
        
        [Authorize(Roles = "SystemAdmin, Agent")]
        public async Task<IActionResult> ViewerAsAgent(int EmployeeId)
        {
            var LogUserId = (await _userManager.GetUserAsync(User)).Id;
            var AppUser = _context.AppUsers.Where(u => u.LogUserId == LogUserId).Include(u => u.Building).FirstOrDefault();
            if(AppUser.Type != API.Enums.UserType.SystemAdmin)
            {
                EmployeeId = AppUser.Id;
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../" + AppUser.Building.Path.Substring(0, AppUser.Building.Path.Length - 3) + "wexBIM");
            }
            else if(EmployeeId != 0)
            {
                string BuildingPath = _context.AppUsers.Where(u => u.Id == EmployeeId).Include(u => u.Building).FirstOrDefault().Building.Path;
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../" + BuildingPath.Substring(0, BuildingPath.Length - 3) + "wexBIM");
            }
            else if(EmployeeId == 0 || AppUser.BuildingId == null)
            {
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../data/SampleHouse.wexbim");
            }

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

        public async Task fillViewerParamDic(int EmployeeId, Dictionary<int, ViewerParameter> viewerParamDic, List<API.Models.Task> Tasks)
        {
            var AgentTasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed).Include(t => t.Incident).ThenInclude(i => i.Asset).ToListAsync();
            Tasks.AddRange(AgentTasks);
            //viewerParam = fillViewParameterForAgents(AgentTasks, viewerParam);
            viewerParamDic.Add(EmployeeId, fillViewParameterForAgents(AgentTasks));
        }

        [Authorize(Roles = "SystemAdmin, Inspector")]
        public async Task<IActionResult> ViewerAsInspector(int InspectorId)
        {
            var LogUserId = (await _userManager.GetUserAsync(User)).Id;
            var AppUser = _context.AppUsers.Where(u => u.LogUserId == LogUserId).Include(u => u.Building).FirstOrDefault();
            if (AppUser.Type != API.Enums.UserType.SystemAdmin)
            {
                InspectorId = AppUser.Id;
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../" + AppUser.Building.Path.Substring(0, AppUser.Building.Path.Length - 3) + "wexBIM");
            }
            else if (InspectorId != 0)
            {
                string BuildingPath = _context.AppUsers.Where(u => u.Id == InspectorId).Include(u => u.Building).FirstOrDefault().Building.Path;
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../" + BuildingPath.Substring(0, BuildingPath.Length - 3) + "wexBIM");
            }
            else if (InspectorId == 0 || AppUser.BuildingId == null)
            {
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../data/SampleHouse.wexbim");
            }
            
            var Agents = await _context.AppUsers.Where(u => u.SuperId == InspectorId).ToListAsync();
            //ViewerParameter viewerParam = new ViewerParameter();
            Dictionary<int, ViewerParameter> viewerParamDic = new Dictionary<int, ViewerParameter>();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var a in Agents)
            {
                //int EmployeeId = a.Id;
                //var AgentTasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed).Include(t => t.Incident).ThenInclude(i => i.Asset).ToListAsync();
                //Tasks.AddRange(AgentTasks);
                ////viewerParam = fillViewParameterForAgents(AgentTasks, viewerParam);
                //viewerParamDic.Add(EmployeeId, fillViewParameterForAgents(AgentTasks));
                await fillViewerParamDic(a.Id, viewerParamDic, Tasks);
            }
            ViewBag.Tasks = Tasks;
            ViewBag.Agents = Agents; 
            return View(viewerParamDic);
        }
        
        public async Task<List<API.Models.AppUser>> fillInspectorAgentsDic(int Id,Dictionary<int, int[]> InspectorAgentsDic, List<API.Models.AppUser> Agents)
        {
            var InspectorAgents = await _context.AppUsers.Where(u => u.SuperId == Id).ToListAsync();
            Agents.AddRange(InspectorAgents);
            InspectorAgentsDic.Add(Id, InspectorAgents.Select(a => a.Id).ToArray());
            return InspectorAgents;
        }

        [Authorize(Roles = "SystemAdmin, Supervisor")]
        public async Task<IActionResult> ViewerAsSupervisor(int SupervisorId)
        {
            var LogUserId = (await _userManager.GetUserAsync(User)).Id;
            var AppUser = _context.AppUsers.Where(u => u.LogUserId == LogUserId).Include(u => u.Building).FirstOrDefault();
            if (AppUser.Type != API.Enums.UserType.SystemAdmin)
            {
                SupervisorId = AppUser.Id;
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../" + AppUser.Building.Path.Substring(0, AppUser.Building.Path.Length - 3) + "wexBIM");
            }
            else if (SupervisorId != 0)
            {
                string BuildingPath = _context.AppUsers.Where(u => u.Id == SupervisorId).Include(u => u.Building).FirstOrDefault().Building.Path;
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../" + BuildingPath.Substring(0, BuildingPath.Length - 3) + "wexBIM");
            }
            else if (SupervisorId == 0 || AppUser.BuildingId == null)
            {
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../data/SampleHouse.wexbim");
            }

            List<API.Models.AppUser> Agents = new List<API.Models.AppUser>();
            Dictionary<int, int[]> InspectorAgentsDic = new Dictionary<int, int[]>();
            var Inspectors = await _context.AppUsers.Where(u => u.SuperId == SupervisorId).ToListAsync();
            //ViewerParameter viewerParam = new ViewerParameter();
            Dictionary<int, ViewerParameter> viewerParamDic = new Dictionary<int, ViewerParameter>();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var i in Inspectors)
            {
                //var InspectorAgents = await _context.AppUsers.Where(u => u.SuperId == i.Id).ToListAsync();
                //Agents.AddRange(InspectorAgents);
                //InspectorAgentsDic.Add(i.Id, InspectorAgents.Select(a => a.Id).ToArray());
                var InspectorAgents = await fillInspectorAgentsDic(i.Id, InspectorAgentsDic, Agents);
                foreach (var a in InspectorAgents)
                {
                    await fillViewerParamDic(a.Id, viewerParamDic, Tasks);
                }
            }
            ViewBag.Tasks = Tasks;
            ViewBag.InspectorAgentsDic = InspectorAgentsDic;
            ViewBag.Agents = Agents;
            ViewBag.Inspectors = Inspectors;
            return View(viewerParamDic);
        }

        public async Task<List<API.Models.AppUser>> fillSupervisorInspectorsDic(int Id, Dictionary<int, int[]> SupervisorInspectorsDic, List<API.Models.AppUser> Inspectors)
        {
            var SupervisorInspectors = await _context.AppUsers.Where(u => u.SuperId == Id).ToListAsync();
            Inspectors.AddRange(SupervisorInspectors);
            SupervisorInspectorsDic.Add(Id, SupervisorInspectors.Select(i => i.Id).ToArray());
            return SupervisorInspectors;
        }

        [Authorize(Roles = "SystemAdmin, Manager")]
        public async Task<IActionResult> ViewerAsManager(int ManagerId)
        {
            var LogUserId = (await _userManager.GetUserAsync(User)).Id;
            var AppUser = _context.AppUsers.Where(u => u.LogUserId == LogUserId).Include(u => u.Building).FirstOrDefault();
            if (AppUser.Type != API.Enums.UserType.SystemAdmin)
            {
                ManagerId = AppUser.Id;
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../" + AppUser.Building.Path.Substring(0, AppUser.Building.Path.Length - 3) + "wexBIM");
            }
            else if (ManagerId != 0)
            {
                string BuildingPath = _context.AppUsers.Where(u => u.Id == ManagerId).Include(u => u.Building).FirstOrDefault().Building.Path;
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../" + BuildingPath.Substring(0, BuildingPath.Length - 3) + "wexBIM");
            }
            else if (ManagerId == 0 || AppUser.BuildingId == null)
            {
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../data/SampleHouse.wexbim");
            }

            List<API.Models.AppUser> Agents = new List<API.Models.AppUser>();
            List<API.Models.AppUser> Inspectors = new List<API.Models.AppUser>();
            Dictionary<int, int[]> InspectorAgentsDic = new Dictionary<int, int[]>();
            Dictionary<int, int[]> SupervisorInspectorsDic = new Dictionary<int, int[]>();            
            var Supervisors = _context.AppUsers.Where(u => u.SuperId == ManagerId).ToList();
            //ViewerParameter viewerParam = new ViewerParameter();
            Dictionary<int, ViewerParameter> viewerParamDic = new Dictionary<int, ViewerParameter>();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var supervisor in Supervisors)
            {
                //var SupervisorInspectors = await _context.AppUsers.Where(u => u.SuperId == supervisor.Id).ToListAsync();
                //Inspectors.AddRange(SupervisorInspectors);
                //SupervisorInspectorsDic.Add(supervisor.Id, SupervisorInspectors.Select(i => i.Id).ToArray());
                var SupervisorInspectors = await fillSupervisorInspectorsDic(supervisor.Id, SupervisorInspectorsDic, Inspectors);
                foreach (var i in SupervisorInspectors)
                {
                    var InspectorAgents = await fillInspectorAgentsDic(i.Id, InspectorAgentsDic, Agents);
                    foreach (var a in InspectorAgents)
                    {
                        await fillViewerParamDic(a.Id, viewerParamDic, Tasks);
                    }
                }
            }
            ViewBag.Tasks = Tasks;
            ViewBag.InspectorAgentsDic = InspectorAgentsDic;
            ViewBag.SupervisorInspectorsDic = SupervisorInspectorsDic;
            ViewBag.Agents = Agents;
            ViewBag.Inspectors = Inspectors;
            ViewBag.Supervisors = Supervisors;
            return View(viewerParamDic);
        }

        [Authorize(Roles = "SystemAdmin, Owner")]
        public async Task<IActionResult> ViewerAsOwner(int OwnerId)
        {
            var LogUserId = (await _userManager.GetUserAsync(User)).Id;
            var AppUser = _context.AppUsers.Where(u => u.LogUserId == LogUserId).Include(u => u.Building).FirstOrDefault();
            if (AppUser.Type != API.Enums.UserType.SystemAdmin)
            {
                OwnerId = AppUser.Id;
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../" + AppUser.Building.Path.Substring(0, AppUser.Building.Path.Length - 3) + "wexBIM");
            }
            else if (OwnerId != 0)
            {
                string BuildingPath = _context.AppUsers.Where(u => u.Id == OwnerId).Include(u => u.Building).FirstOrDefault().Building.Path;
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../" + BuildingPath.Substring(0, BuildingPath.Length - 3) + "wexBIM");
            }
            else if (OwnerId == 0 || AppUser.BuildingId == null)
            {
                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../data/SampleHouse.wexbim");
            }

            List<API.Models.AppUser> Agents = new List<API.Models.AppUser>();
            List<API.Models.AppUser> Inspectors = new List<API.Models.AppUser>();
            List<API.Models.AppUser> Supervisors = new List<API.Models.AppUser>();
            Dictionary<int, int[]> InspectorAgentsDic = new Dictionary<int, int[]>();
            Dictionary<int, int[]> SupervisorInspectorsDic = new Dictionary<int, int[]>();
            Dictionary<int, int[]> ManagerSupervisorsDic = new Dictionary<int, int[]>();
            var Managers = _context.AppUsers.Where(u => u.SuperId == OwnerId).ToList();
            //ViewerParameter viewerParam = new ViewerParameter();
            Dictionary<int, ViewerParameter> viewerParamDic = new Dictionary<int, ViewerParameter>();
            List<API.Models.Task> Tasks = new List<API.Models.Task>();
            foreach (var Manager in Managers)
            {
                var ManagerSupervisors = _context.AppUsers.Where(u => u.SuperId == Manager.Id).ToList();
                Supervisors.AddRange(ManagerSupervisors);
                ManagerSupervisorsDic.Add(Manager.Id, ManagerSupervisors.Select(s => s.Id).ToArray());
                foreach (var supervisor in ManagerSupervisors)
                {
                    var SupervisorInspectors = await fillSupervisorInspectorsDic(supervisor.Id, SupervisorInspectorsDic, Inspectors);
                    foreach (var i in SupervisorInspectors)
                    {
                        var InspectorAgents = await fillInspectorAgentsDic(i.Id, InspectorAgentsDic, Agents);
                        foreach (var a in InspectorAgents)
                        {
                            await fillViewerParamDic(a.Id, viewerParamDic, Tasks);
                        }
                    }
                }
            }
            ViewBag.Tasks = Tasks;
            ViewBag.InspectorAgentsDic = InspectorAgentsDic;
            ViewBag.SupervisorInspectorsDic = SupervisorInspectorsDic;
            ViewBag.ManagerSupervisorsDic = ManagerSupervisorsDic;
            ViewBag.Agents = Agents;
            ViewBag.Inspectors = Inspectors;
            ViewBag.Supervisors = Supervisors;
            ViewBag.Managers = Managers;
            return View(viewerParamDic);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
