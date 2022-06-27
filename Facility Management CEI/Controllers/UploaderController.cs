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
using Microsoft.AspNetCore.Authorization;
using Facility_Management_CEI.IdentityDb;
using Microsoft.AspNetCore.Identity;
using Facility_Management_CEI.APIs.Models;
using Microsoft.EntityFrameworkCore;

namespace Facility_Management_CEI.Controllers
{
    public class UploaderController : Controller
    {
        private readonly UserManager<LogUser> _userManager;
        public ApplicationDBContext _context { get; set; }
      
        public UploaderController(UserManager<LogUser> userManager, ApplicationDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        
        [Authorize(Roles = "SystemAdmin, Owner")]
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        [RequestSizeLimit(2147483648)]
        public async Task Index(IFormFile file)
        {
            if (file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine("wwwroot\\data", fileName);
                using (Stream fileStream = System.IO.File.Create(path))
                {
                    await file.CopyToAsync(fileStream);
                }
                ConvertToWexBIM(path);
                ViewBag.Message = "File Uploaded Successfully";
                //Populate Database with ifcdata
                new API.Controllers.IFCController(_context).AddBuildingDataFromIFC(fileName);
                // Adding Building to owner
                var LogUserId = (await _userManager.GetUserAsync(User)).Id;
                var AppUser = _context.AppUsers.Where(u => u.LogUserId == LogUserId).FirstOrDefault();
                AppUser.BuildingId = _context.Buildings.Where(b => b.Path == "data/" + fileName).FirstOrDefault().Id;
                var Managers = _context.AppUsers.Where(u => u.SuperId == AppUser.Id).ToList();
                foreach (var Manager in Managers)
                {
                    Manager.BuildingId = AppUser.BuildingId;
                    var ManagerSupervisors = _context.AppUsers.Where(u => u.SuperId == Manager.Id).ToList();
                    foreach (var supervisor in ManagerSupervisors)
                    {
                        supervisor.BuildingId = AppUser.BuildingId;
                        var SupervisorInspectors = _context.AppUsers.Where(u => u.SuperId == supervisor.Id).ToList();
                        foreach (var inspector in SupervisorInspectors)
                        {
                            inspector.BuildingId = AppUser.BuildingId;
                            var InspectorAgents = _context.AppUsers.Where(u => u.SuperId == inspector.Id).ToList();
                            foreach (var agent in InspectorAgents)
                            {
                                agent.BuildingId = AppUser.BuildingId;
                            }
                        }
                    }
                }
                _context.SaveChanges();
            }
            //return RedirectToAction("ViewerAsOwner", "Viewer");
        }

        [HttpPost]
        public IActionResult ClearDatabase()
        {
            new API.Controllers.IFCController(_context).ClearDatabase();
            return RedirectToAction("Login", "Account");
        }

        void ConvertToWexBIM(string filePath)
        {
            //  const string fileName = "../../my.ifc";
            using (var model = IfcStore.Open(filePath))
            {
                var context = new Xbim3DModelContext(model);
                context.CreateContext();

                // physical full path in drive
                var wexBimFullPath = Path.ChangeExtension(filePath, "wexBIM");

                var wexBimFileName = Path.GetFileName(wexBimFullPath);

                //ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../data/" + wexBimFileName);

                using (var wexBiMfile = System.IO.File.Create(wexBimFullPath))
                {
                    using (var wexBimBinaryWriter = new BinaryWriter(wexBiMfile))
                    {
                        model.SaveAsWexBim(wexBimBinaryWriter);
                        wexBimBinaryWriter.Close();
                    }
                    wexBiMfile.Close();
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
