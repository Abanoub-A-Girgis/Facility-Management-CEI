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
        public int ManagerId;
        public int SuperVisorId;
        public int InspectorId;
        public int UserId;
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
        
        public async Task<IActionResult> ViewerAsAgent(/*int EmployeeId*/)
        {
            //ViewerParam.UserId;
            var Tasks = await _context.Tasks.Where(t => t.AssignedToId == 4).FirstOrDefaultAsync();
            return View();
        }

        [HttpPost]
        public IActionResult ViewerAsInspector(int id)
        {
            //ViewerParam.UserId;
            return View();
        }

        [HttpPost]
        public IActionResult ViewerAsSupervisor(int id)
        {
            //ViewerParam.UserId;
            return View();
        }

        [HttpPost]
        public IActionResult ViewerAsManager(int id)
        {
            //ViewerParam.UserId;
            return View();
        }

        [HttpPost]
        public IActionResult ViewerAsOwner(int id)
        {
            //ViewerParam.UserId;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
