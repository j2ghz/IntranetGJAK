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
            System.Diagnostics.Debug.WriteLine("File upload processing complete");
            List<IReturnData> files = new List<IReturnData>();
            foreach (var file in Request.Form.Files)
            {
                var fileresult = new ViewDataUploadFilesResult();

                try
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    fileresult.name = fileName;
                    fileresult.size = file.Length;

                    var filePath = Path.Combine(_hostingEnvironment.ApplicationBasePath, "wwwroot", "Uploads", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    await file.SaveAsAsync(filePath);

                    fileresult.url = "/Uploads/" + fileName;
                    fileresult.thumbnail_url = Tools.Thumbnails.GetThumbnail(filePath);
                    fileresult.deleteUrl = "/?name=" + fileName;
                    fileresult.deleteType = "DELETE";
                }
                catch (Exception ex)
                {
                    ViewDataUploadError error = new ViewDataUploadError()
                    {
                        name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'),
                        size = file.Length,
                        error = ex.ToString()
                    };
                }
                finally
                {
                    System.Diagnostics.Debug.WriteLine(fileresult.name + " - " + fileresult.size);
                    files.Add(fileresult);
                }
            }
            ReturnData data = new ReturnData();
            data.files = files;
            System.Diagnostics.Debug.WriteLine("File upload processing complete");
            return Json(data);
        }

        [HttpDelete]
        public async Task<IActionResult> Index(string name)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> ListFiles()
        {
            System.Diagnostics.Debug.WriteLine("Listing files");
            List<IReturnData> files = new List<IReturnData>();
            foreach (string filepath in Directory.EnumerateFiles(Path.Combine(_hostingEnvironment.ApplicationBasePath, "wwwroot", "Uploads")))
            {
                var fileresult = new ViewDataUploadFilesResult();
                try
                {
                    FileInfo file = new FileInfo(filepath);
                    fileresult.name = file.Name;
                    fileresult.size = file.Length;

                    fileresult.url = "/Uploads/" + file.Name;
                    fileresult.thumbnail_url = Tools.Thumbnails.GetThumbnail(filepath);
                    fileresult.deleteUrl = "/?name=" + file.Name;
                    fileresult.deleteType = "DELETE";
                }
                catch (Exception ex)
                {
                    FileInfo file = new FileInfo(filepath);
                    ViewDataUploadError error = new ViewDataUploadError()
                    {
                        name = file.Name,
                        size = file.Length,
                        error = ex.ToString()
                    };
                }
                finally
                {
                    System.Diagnostics.Debug.WriteLine(fileresult.name + " - " + fileresult.size);
                    files.Add(fileresult);
                }
            }
            ReturnData data = new ReturnData();
            data.files = files;
            System.Diagnostics.Debug.WriteLine("File listing complete");
            return Json(data);
        }
    }

    public class ReturnData
    {
        public IList<IReturnData> files { get; set; }
    }

    public class ViewDataUploadFilesResult : IReturnData
    {
        public string name { get; set; }
        public long size { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string deleteUrl { get; set; }
        public string deleteType { get; set; }
    }

    public class ViewDataUploadError : IReturnData
    {
        public string name { get; set; }
        public long size { get; set; }

        public string error { get; set; }
    }

    public interface IReturnData
    {
        string name { get; set; }
        long size { get; set; }
    }
}