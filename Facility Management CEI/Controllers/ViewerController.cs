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
using Microsoft.AspNetCore.Authorization;
using Facility_Management_CEI.APIs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public List<int> Rooms = new List<int>();
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
                    else
                        viewerParam.Rooms.Add((int)t.Incident.SpaceId);
                }
                else if (t.Priority == API.Enums.Priority.High)
                {
                    if (t.Incident.AssetId != null)
                        viewerParam.High.Add((int)t.Incident.AssetId);
                    else
                        viewerParam.Rooms.Add((int)t.Incident.SpaceId);
                }
                else if (t.Priority == API.Enums.Priority.Medium)
                {
                    if (t.Incident.AssetId != null)
                        viewerParam.Medium.Add((int)t.Incident.AssetId);
                    else
                        viewerParam.Rooms.Add((int)t.Incident.SpaceId);
                }
                else if (t.Priority == API.Enums.Priority.Low)
                {
                    if (t.Incident.AssetId != null)
                        viewerParam.Low.Add((int)t.Incident.AssetId);
                    else
                        viewerParam.Rooms.Add((int)t.Incident.SpaceId);
                }
            }
            return viewerParam;
        }
        
        [Authorize(Roles = "AccountManager, Agent")]
        public async Task<IActionResult> ViewerAsAgent(int Id)
        {
            var LogUserId = (await _userManager.GetUserAsync(User)).Id;
            var AppUser = _context.AppUsers.Where(u => u.LogUserId == LogUserId).Include(u => u.Building).FirstOrDefault();
            if(AppUser.Type != API.Enums.UserType.AccountManager)
            {
                Id = AppUser.Id;
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/" + AppUser.Building.Path.Substring(0, AppUser.Building.Path.Length - 3) + "wexBIM");
                ViewBag.WexBIMPaths = _context.Floors.OrderBy(f => f.Path).ToList();
            }
            else if(Id != 0)
            {
                API.Enums.UserType EmployeeType = new API.Enums.UserType();
                try
                {
                    EmployeeType = _context.AppUsers.Where(u => u.Id == Id).FirstOrDefault().Type;
                }
                catch
                {
                    TempData["message"] = "Sorry, You have entered an invalid user ID";
                    return RedirectToAction("ErrorGeneric", "ErrorPages");
                }
                if (EmployeeType != API.Enums.UserType.Agent)
                { 
                    TempData["message"] = "Sorry, You have entered an invalid user ID";
                    return RedirectToAction("ErrorGeneric", "ErrorPages");
                }
                string BuildingPath = _context.AppUsers.Where(u => u.Id == Id).Include(u => u.Building).FirstOrDefault().Building.Path;
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/" + BuildingPath.Substring(0, BuildingPath.Length - 3) + "wexBIM");
                ViewBag.WexBIMPaths = _context.Floors.OrderBy(f => f.Path).ToList();
            }
            else if(Id == 0 || AppUser.BuildingId == null)
            {
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/data/SampleHouse.wexbim");
                ViewBag.WexBIMPaths = new List<API.Models.Floor>() {
                   new API.Models.Floor() { Id = 0, FloorName = "Samplehouse", Path = "/data/SampleHouse.wexbim" }
                };
            }

            var Tasks = await _context.Tasks.Where(t => t.AssignedToId == Id && t.Status != API.Enums.TaskStatus.Completed)
                .Include(t => t.Incident.Asset)
                .Include(t => t.Incident.Space)
                .ThenInclude(s => s.Floor)
                .ToListAsync();
            ViewerParameter viewerParam = fillViewParameterForAgents(Tasks);

            ViewBag.Ids = _context.AppUsers.Where(u => u.Type == API.Enums.UserType.Agent).Select(u => new { FullText = u.Id + ": " + u.FirstName + " " + u.LastName, Id = u.Id }).ToList();
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
            var AgentTasks = await _context.Tasks.Where(t => t.AssignedToId == EmployeeId && t.Status != API.Enums.TaskStatus.Completed)
                .Include(t => t.Incident.Asset)
                .Include(t => t.Incident.Space)
                .ThenInclude(s => s.Floor)
                .ToListAsync();
            Tasks.AddRange(AgentTasks);
            //viewerParam = fillViewParameterForAgents(AgentTasks, viewerParam);
            viewerParamDic.Add(EmployeeId, fillViewParameterForAgents(AgentTasks));
        }

        [Authorize(Roles = "AccountManager, Inspector")]
        public async Task<IActionResult> ViewerAsInspector(int Id)
        {
            var LogUserId = (await _userManager.GetUserAsync(User)).Id;
            var AppUser = _context.AppUsers.Where(u => u.LogUserId == LogUserId).Include(u => u.Building).FirstOrDefault();
            if (AppUser.Type != API.Enums.UserType.AccountManager)
            {
                Id = AppUser.Id;
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/" + AppUser.Building.Path.Substring(0, AppUser.Building.Path.Length - 3) + "wexBIM");
                ViewBag.WexBIMPaths = _context.Floors.OrderBy(f => f.Path).ToList();
            }
            else if (Id != 0)
            {
                API.Enums.UserType EmployeeType = new API.Enums.UserType();
                try
                {
                    EmployeeType = _context.AppUsers.Where(u => u.Id == Id).FirstOrDefault().Type;
                }
                catch
                {
                    TempData["message"] = "Sorry, You have entered an invalid user ID";
                    return RedirectToAction("ErrorGeneric", "ErrorPages");
                }
                if (EmployeeType != API.Enums.UserType.Inspector)
                {
                    TempData["message"] = "Sorry, You have entered an invalid user ID";
                    return RedirectToAction("ErrorGeneric", "ErrorPages");
                }
                string BuildingPath = _context.AppUsers.Where(u => u.Id == Id).Include(u => u.Building).FirstOrDefault().Building.Path;
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/" + BuildingPath.Substring(0, BuildingPath.Length - 3) + "wexBIM");
                ViewBag.WexBIMPaths = _context.Floors.OrderBy(f => f.Path).ToList();
            }
            else if (Id == 0 || AppUser.BuildingId == null)
            {
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/data/SampleHouse.wexbim");
                ViewBag.WexBIMPaths = new List<API.Models.Floor>() {
                   new API.Models.Floor() { Id = 0, FloorName = "Samplehouse", Path = "/data/SampleHouse.wexbim" }
                };
            }
            
            var Agents = await _context.AppUsers.Where(u => u.SuperId == Id).ToListAsync();
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

            ViewBag.Ids = _context.AppUsers.Where(u => u.Type == API.Enums.UserType.Inspector).Select(u => new { FullText = u.Id + ": " + u.FirstName + " " + u.LastName, Id = u.Id }).ToList();
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

        [Authorize(Roles = "AccountManager, Supervisor")]
        public async Task<IActionResult> ViewerAsSupervisor(int Id)
        {
            var LogUserId = (await _userManager.GetUserAsync(User)).Id;
            var AppUser = _context.AppUsers.Where(u => u.LogUserId == LogUserId).Include(u => u.Building).FirstOrDefault();
            if (AppUser.Type != API.Enums.UserType.AccountManager)
            {
                Id = AppUser.Id;
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/" + AppUser.Building.Path.Substring(0, AppUser.Building.Path.Length - 3) + "wexBIM");
                ViewBag.WexBIMPaths = _context.Floors.OrderBy(f => f.Path).ToList();
            }
            else if (Id != 0)
            {
                API.Enums.UserType EmployeeType = new API.Enums.UserType();
                try
                {
                    EmployeeType = _context.AppUsers.Where(u => u.Id == Id).FirstOrDefault().Type;
                }
                catch
                {
                    TempData["message"] = "Sorry, You have entered an invalid user ID";
                    return RedirectToAction("ErrorGeneric", "ErrorPages");
                }
                if (EmployeeType != API.Enums.UserType.Supervisor)
                {
                    TempData["message"] = "Sorry, You have entered an invalid user ID";
                    return RedirectToAction("ErrorGeneric", "ErrorPages");
                }
                string BuildingPath = _context.AppUsers.Where(u => u.Id == Id).Include(u => u.Building).FirstOrDefault().Building.Path;
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/" + BuildingPath.Substring(0, BuildingPath.Length - 3) + "wexBIM");
                ViewBag.WexBIMPaths = _context.Floors.OrderBy(f => f.Path).ToList();
            }
            else if (Id == 0 || AppUser.BuildingId == null)
            {
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/data/SampleHouse.wexbim");
                ViewBag.WexBIMPaths = new List<API.Models.Floor>() {
                   new API.Models.Floor() { Id = 0, FloorName = "Samplehouse", Path = "/data/SampleHouse.wexbim" }
                };
            }

            List<API.Models.AppUser> Agents = new List<API.Models.AppUser>();
            Dictionary<int, int[]> InspectorAgentsDic = new Dictionary<int, int[]>();
            var Inspectors = await _context.AppUsers.Where(u => u.SuperId == Id).ToListAsync();
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

            ViewBag.Ids = _context.AppUsers.Where(u => u.Type == API.Enums.UserType.Supervisor).Select(u => new { FullText = u.Id + ": " + u.FirstName + " " + u.LastName, Id = u.Id }).ToList();
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

        [Authorize(Roles = "AccountManager, Manager")]
        public async Task<IActionResult> ViewerAsManager(int Id)
        {
            var LogUserId = (await _userManager.GetUserAsync(User)).Id;
            var AppUser = _context.AppUsers.Where(u => u.LogUserId == LogUserId).Include(u => u.Building).FirstOrDefault();
            if (AppUser.Type != API.Enums.UserType.AccountManager)
            {
                Id = AppUser.Id;
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/" + AppUser.Building.Path.Substring(0, AppUser.Building.Path.Length - 3) + "wexBIM");
                ViewBag.WexBIMPaths = _context.Floors.OrderBy(f => f.Path).ToList();
            }
            else if (Id != 0)
            {
                API.Enums.UserType EmployeeType = new API.Enums.UserType();
                try
                {
                    EmployeeType = _context.AppUsers.Where(u => u.Id == Id).FirstOrDefault().Type;
                }
                catch
                {
                    TempData["message"] = "Sorry, You have entered an invalid user ID";
                    return RedirectToAction("ErrorGeneric", "ErrorPages");
                }
                if (EmployeeType != API.Enums.UserType.Manager)
                {
                    TempData["message"] = "Sorry, You have entered an invalid user ID";
                    return RedirectToAction("ErrorGeneric", "ErrorPages");
                }
                string BuildingPath = _context.AppUsers.Where(u => u.Id == Id).Include(u => u.Building).FirstOrDefault().Building.Path;
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/" + BuildingPath.Substring(0, BuildingPath.Length - 3) + "wexBIM");
                ViewBag.WexBIMPaths = _context.Floors.OrderBy(f => f.Path).ToList();
            }
            else if (Id == 0 || AppUser.BuildingId == null)
            {
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/data/SampleHouse.wexbim");
                ViewBag.WexBIMPaths = new List<API.Models.Floor>() {
                   new API.Models.Floor() { Id = 0, FloorName = "Samplehouse", Path = "/data/SampleHouse.wexbim" }
                };
            }

            List<API.Models.AppUser> Agents = new List<API.Models.AppUser>();
            List<API.Models.AppUser> Inspectors = new List<API.Models.AppUser>();
            Dictionary<int, int[]> InspectorAgentsDic = new Dictionary<int, int[]>();
            Dictionary<int, int[]> SupervisorInspectorsDic = new Dictionary<int, int[]>();            
            var Supervisors = _context.AppUsers.Where(u => u.SuperId == Id).ToList();
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

            ViewBag.Ids = _context.AppUsers.Where(u => u.Type == API.Enums.UserType.Manager).Select(u => new { FullText = u.Id + ": " + u.FirstName + " " + u.LastName, Id = u.Id }).ToList();
            ViewBag.Tasks = Tasks;
            ViewBag.InspectorAgentsDic = InspectorAgentsDic;
            ViewBag.SupervisorInspectorsDic = SupervisorInspectorsDic;
            ViewBag.Agents = Agents;
            ViewBag.Inspectors = Inspectors;
            ViewBag.Supervisors = Supervisors;
            return View(viewerParamDic);
        }

        [Authorize(Roles = "AccountManager, Owner")]
        public async Task<IActionResult> ViewerAsOwner(int Id)
        {
            var LogUserId = (await _userManager.GetUserAsync(User)).Id;
            var AppUser = _context.AppUsers.Where(u => u.LogUserId == LogUserId).Include(u => u.Building).FirstOrDefault();
            if (AppUser.Type != API.Enums.UserType.AccountManager)
            {
                Id = AppUser.Id;
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/" + AppUser.Building.Path.Substring(0, AppUser.Building.Path.Length - 3) + "wexBIM");
                ViewBag.WexBIMPaths = _context.Floors.OrderBy(f => f.Path).ToList();
            }
            else if (Id != 0)
            {
                API.Enums.UserType EmployeeType = new API.Enums.UserType();
                try
                {
                    EmployeeType = _context.AppUsers.Where(u => u.Id == Id).FirstOrDefault().Type;
                }
                catch
                {
                    TempData["message"] = "Sorry, You have entered an invalid user ID";
                    return RedirectToAction("ErrorGeneric", "ErrorPages");
                }
                if (EmployeeType != API.Enums.UserType.Owner)
                {
                    TempData["message"] = "Sorry, You have entered an invalid user ID";
                    return RedirectToAction("ErrorGeneric", "ErrorPages");
                }
                //string BuildingPath = _context.AppUsers.Where(u => u.Id == Id).Include(u => u.Building).FirstOrDefault().Building.Path;
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/" + BuildingPath.Substring(0, BuildingPath.Length - 3) + "wexBIM");
                ViewBag.WexBIMPaths = _context.Floors.OrderBy(f => f.Path).ToList();
            }
            else if (Id == 0 || AppUser.BuildingId == null)
            {
                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "/data/SampleHouse.wexbim");
                ViewBag.WexBIMPaths = new List<API.Models.Floor>() { 
                   new API.Models.Floor() { Id = 0, FloorName = "Samplehouse", Path = "/data/SampleHouse.wexbim" } 
                };
            }

            List<API.Models.AppUser> Agents = new List<API.Models.AppUser>();
            List<API.Models.AppUser> Inspectors = new List<API.Models.AppUser>();
            List<API.Models.AppUser> Supervisors = new List<API.Models.AppUser>();
            Dictionary<int, int[]> InspectorAgentsDic = new Dictionary<int, int[]>();
            Dictionary<int, int[]> SupervisorInspectorsDic = new Dictionary<int, int[]>();
            Dictionary<int, int[]> ManagerSupervisorsDic = new Dictionary<int, int[]>();
            var Managers = _context.AppUsers.Where(u => u.SuperId == Id).ToList();
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

            ViewBag.Ids = _context.AppUsers.Where(u => u.Type == API.Enums.UserType.Owner).Select(u => new { FullText = u.Id + ": " + u.FirstName + " " + u.LastName, Id = u.Id }).ToList();
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
