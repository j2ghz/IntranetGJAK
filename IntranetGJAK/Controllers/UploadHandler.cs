using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Dnx.Runtime;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetGJAK.Controllers
{
    public class UploadHandler : Controller
    {
        private IApplicationEnvironment _hostingEnvironment;

        public UploadHandler(IApplicationEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult test()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IList<IFormFile> files)
        {
            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue
                    .Parse(file.ContentDisposition)
                    .FileName
                    .Trim('"');// FileName returns "fileName.ext"(with double quotes) in beta 3

                if (true)//fileName.EndsWith(".txt"))// Important for security if saving in webroot
                {
                    var filePath = _hostingEnvironment.ApplicationBasePath + "\\wwwroot\\" + fileName;
                    await file.SaveAsAsync(filePath);
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}