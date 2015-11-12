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
    public class Files : Controller
    {
        private IApplicationEnvironment _hostingEnvironment;

        public Files(IApplicationEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Index(IList<IFormFile> files)
        {
            List<ViewDataUploadFilesResult> data = new List<ViewDataUploadFilesResult>();
            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
                var filePath = Path.Combine(_hostingEnvironment.ApplicationBasePath, "\\wwwroot\\Uploads\\" + fileName);
                await file.SaveAsAsync(filePath);
                var fileresult = new ViewDataUploadFilesResult()
                {
                    name = fileName,
                    size = file.Length,
                    url = "/Files/" + fileName
                };
                data.Add(fileresult);
            }

            return Json(data);
        }

        [HttpDelete]
        public async Task<IActionResult> Index(string filename)
        {
            throw new NotImplementedException();
        }
    }

    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public long size { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
    }
}