using IntranetGJAK.Tools;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Dnx.Runtime;
using Microsoft.Framework.Logging;
using Microsoft.Net.Http.Headers;
using Serilog;
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
            Log.Debug("File handler created with base path: {@basepath}", Path.Combine(hostingEnvironment.ApplicationBasePath, "wwwroot", "Uploads"));
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> Upload()
        {
            var log = Log.ForContext("ACtion", "Upload");
            log.Information("Starting file upload processing from {@source}, number of files attached: {@filesAttached}", Request.Host.ToString(), Request.Form.Files.Count);

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

                    Task savefile = file.SaveAsAsync(filePath);

                    fileresult.url = "/Uploads/" + fileName;
                    fileresult.thumbnail_url = Tools.Thumbnails.GetThumbnail(fileName);
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
            Log.Information("Completed file upload processing from {@source}, processed {@filesProcessed} out of {@filesAttached} files", Request.Host.ToString(), data.files.Count, Request.Form.Files.Count);
            Log.Verbose("Response {@fileData}", data.files);
            return Json(data);
        }

        [HttpDelete]
        [ActionName("Index")]
        public async Task<IActionResult> Delete(string name)
        {
            Log.Information("Starting deletion of {@fileName}", name);
            ReturnDeleteData data = new ReturnDeleteData();
            data.files = new Dictionary<string, bool>();
            try
            {
                FileInfo file = new FileInfo(Path.Combine(_hostingEnvironment.ApplicationBasePath, "wwwroot", "Uploads", name));
                if (file.Exists == true)
                    await file.DeleteAsync();
                if (file.Exists == false)
                    throw new Exception("File not deleted!");
                else
                    Log.Information("File {@fileName} successfully deleted", file.Name);
                data.files.Add(name, true);
            }
            catch (Exception ex)
            {
                data.files.Add(name, false);
                Log.Warning("File deletion failed. {@Exception}", ex);
            }
            Log.Information("Finished deletion of {@fileName}", name);
            return Json(data);
        }

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> List()
        {
            Log.Information("Starting file listing for {@source}", Request.Host.ToString());
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
                    Log.Information("Found file: {@FileName} {@Size}", fileresult.name, Formatting.FormatBytes(fileresult.size));
                    files.Add(fileresult);
                }
            }
            ReturnData data = new ReturnData();
            data.files = files;
            Log.Information("Finished file listing for {@source}", Request.Host.ToString());
            return Json(data);
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