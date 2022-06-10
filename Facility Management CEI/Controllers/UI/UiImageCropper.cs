using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Skote.UI
{
    public class UiImageCropper : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("ui-image-cropper")]
        public ActionResult uiimagecropper()
        {
            return View();
        }

    }
}
