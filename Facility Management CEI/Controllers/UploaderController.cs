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

namespace Facility_Management_CEI.Controllers
{
    public class UploaderController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public UploaderController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public ActionResult Uploader()
        {

            return View();
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        [RequestSizeLimit(2147483648)]
        [Authorize(Roles = "SystemAdmin,Owner")]
        public async Task<IActionResult> Uploader(IFormFile file)
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
            }

            return RedirectToAction("Viewer");
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

                ConfigurationManager.AppSettings.Set("wexBIMFullPath", "../data/" + wexBimFileName);

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
