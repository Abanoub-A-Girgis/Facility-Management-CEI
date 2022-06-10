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

namespace Facility_Management_CEI.Controllers
{
    public class ViewerParameter
    {
        public int UserId;
        public List<int> Severe = new List<int>();
        public List<int> High = new List<int>();
        public List<int> Medium = new List<int>();
        public List<int> Low = new List<int>();
    }
    
    public class ViewerController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ViewerController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult ViewerTest()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Viewer(ViewerParameter ViewerParam)
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
