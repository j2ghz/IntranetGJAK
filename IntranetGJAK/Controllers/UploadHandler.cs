using IntranetGJAK.Tools;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Dnx.Runtime;
using Microsoft.Net.Http.Headers;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IntranetGJAK.Controllers
{
    public class Files : Controller
    {
        private IApplicationEnvironment _hostingEnvironment;
        private string FileUploadPath;

        public Files(IApplicationEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            FileUploadPath = Path.Combine(hostingEnvironment.ApplicationBasePath, "Uploads");
            Log.Verbose("File handler created with base path: {@basepath}", FileUploadPath);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> Upload()
        {
            IFormCollection Form = await Request.ReadFormAsync();
            ILogger log = Log.ForContext("User", User.Identity.Name);
            log.Information("Starting file upload processing, number of files attached: {@filesAttached}", Form.Files.Count);

            List<IReturnData> files = new List<IReturnData>();
            foreach (var file in Form.Files)
            {
                var fileresult = new ViewDataUploadFilesResult();

                try
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    fileresult.name = fileName;
                    fileresult.size = file.Length;

                    var filePath = Path.Combine(FileUploadPath, fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    Task savefile = file.SaveAsAsync(filePath);

                    fileresult.url = "/Files/Download/?name=" + fileName;
                    fileresult.thumbnailUrl = Thumbnails.GetThumbnail(fileName);
                    fileresult.deleteUrl = "/Files/Index/?name=" + fileName;
                    fileresult.deleteType = "DELETE";

                    files.Add(fileresult);

                    await savefile;
                }
                catch (Exception ex)
                {
                    ViewDataUploadError error = new ViewDataUploadError()
                    {
                        name = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'),
                        size = file.Length,
                        error = ex.ToString()
                    };
                    log.Warning("Processing error: {@Exception}", ex);
                    files.Add(error);
                }
                finally
                {
                    log.Information("Processed file: {@fileName} {@fileSize}", fileresult.name, Formatting.FormatBytes(fileresult.size));
                }
            }
            ReturnData data = new ReturnData();
            data.files = files;
            log.Information("Completed file upload processing, processed {@filesProcessed} out of {@filesAttached} files", data.files.Count, Form.Files.Count);
            log.Verbose("Response {@fileData}", data.files);
            return Json(data);
        }

        [HttpDelete]
        [ActionName("Index")]
        public async Task<IActionResult> Delete(string name)
        {
            ILogger log = Log.ForContext("User", User.Identity.Name);
            log.Information("Starting deletion of {@fileName}", name);
            ReturnDeleteData data = new ReturnDeleteData();
            data.files = new Dictionary<string, bool>();
            try
            {
                FileInfo file = new FileInfo(Path.Combine(FileUploadPath, name));
                if (file.Exists == true)
                    await file.DeleteAsync();
                if (file.Exists == false)
                    throw new Exception("File not deleted!");
                else
                    log.Information("File {@fileName} successfully deleted", file.Name);
                data.files.Add(name, true);
            }
            catch (Exception ex)
            {
                data.files.Add(name, false);
                log.Warning("File deletion failed. {@Exception}", ex);
            }
            log.Information("Finished deletion of {@fileName}", name);
            return Json(data);
        }

        [HttpGet]
        [ActionName("Index")]
        public IActionResult List()
        {
            ILogger log = Log.ForContext("User", User.Identity.Name);
            log.Information("Starting file listing");
            List<IReturnData> files = new List<IReturnData>();
            foreach (string filepath in Directory.EnumerateFiles(FileUploadPath))
            {
                var fileresult = new ViewDataUploadFilesResult();
                try
                {
                    FileInfo file = new FileInfo(filepath);
                    fileresult.name = file.Name;
                    fileresult.size = file.Length;

                    fileresult.url = "/Files/Download/?name=" + file.Name;
                    fileresult.thumbnailUrl = Tools.Thumbnails.GetThumbnail(filepath);
                    fileresult.deleteUrl = "/Files/Index/?name=" + file.Name;
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
                    log.Information("Found file: {@FileName} {@Size}", fileresult.name, Formatting.FormatBytes(fileresult.size));
                    files.Add(fileresult);
                }
            }
            ReturnData data = new ReturnData();
            data.files = files;
            log.Information("Finished file listing");
            return Json(data);
        }

        [HttpGet]
        [ActionName("Download")]
        public IActionResult Download(string name)
        {
            FileInfo file = new FileInfo(Path.Combine(FileUploadPath, name));
            return File(file.OpenRead(), "application/octet-stream", file.Name);
        }
    }

    public class ReturnDeleteData
    {
        public Dictionary<string, bool> files;
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
        public string thumbnailUrl { get; set; }
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