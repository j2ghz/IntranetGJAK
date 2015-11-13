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
        public async Task<IActionResult> Index()
        {
            Console.Write(DateTime.Now);
            Console.WriteLine(Request.Form.Files.Count);
            List<ViewDataUploadFilesResult> files = new List<ViewDataUploadFilesResult>();
            foreach (var file in Request.Form.Files)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var filePath = Path.Combine(_hostingEnvironment.ApplicationBasePath, "wwwroot", "Uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                await file.SaveAsAsync(filePath);
                var fileresult = new ViewDataUploadFilesResult()
                {
                    name = fileName,
                    size = file.Length,
                    url = "/Uploads/" + fileName
                };
                files.Add(fileresult);
                Console.Write(DateTime.Now);
                Console.WriteLine(fileName);
            }
            ReturnData data = new ReturnData();
            data.files = files;
            return Json(data);
        }

        [HttpDelete]
        public async Task<IActionResult> Index(string filename)
        {
            throw new NotImplementedException();
        }
    }

    public class ReturnData
    {
        public IList<ViewDataUploadFilesResult> files { get; set; }
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