// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileController.cs" company="Jozef Holly">
//   Copyright
// </copyright>
// <summary>
//   Controller for WebAPI used for uploading and downloading files
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace IntranetGJAK.Controllers
{
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using IntranetGJAK.Models;
    using IntranetGJAK.Tools;

    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.Net.Http.Headers;

    using Serilog;

    /// <summary>
    /// Controller for WebAPI used for uploading and downloading files
    /// </summary>
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        /// <summary>
        /// The file upload path.
        /// </summary>
        private readonly string fileUploadPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">
        /// The hosting environment.
        /// </param>
        public FileController(IApplicationEnvironment hostingEnvironment)
        {
            this.fileUploadPath = Path.Combine(hostingEnvironment.ApplicationBasePath, "Uploads");
            Log.Verbose("File handler created with base path: {@basepath}", this.fileUploadPath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">
        /// The hosting environment.
        /// </param>
        /// <param name="files">A list of files</param>
        public FileController(IApplicationEnvironment hostingEnvironment, IFileRepository files)
        {
            this.Files = files;
            this.fileUploadPath = Path.Combine(hostingEnvironment.ApplicationBasePath, "Uploads");
            Log.Verbose("File handler created with base path: {@basepath}", this.fileUploadPath);
        }

        /// <summary>
        /// Gets or sets a list of all files from database
        /// </summary>
        [FromServices]
        // ReSharper disable once MemberCanBePrivate.Global
        public IFileRepository Files { get; set; }

        /// <summary>
        /// A list of all files encoded loaded from database
        /// </summary>
        /// <returns>JSON for BlueImpFileUpload plugin</returns>
        [HttpGet]
        public JsonResult Get()
        {
            Log.Information("Get triggerd");
            return this.Json(this.Files.GetAll()); // TODO: return the right format
        }

        /// <summary>
        /// Download a single file
        /// </summary>
        /// <param name="id">ID of file from database</param>
        /// <returns>File to be sent to client</returns>
        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(string id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.HttpUnauthorized();
            }

            var item = this.Files.Find(id);
            if (item == null)
            {
                return this.HttpNotFound("File not found in database");
            }

            var file = new FileInfo(item.Path);
            if (!file.Exists)
            {
                return this.HttpNotFound("File found in database, but not on disk");
            }

            return this.PhysicalFile(file.FullName, "application/octet-stream"); // copypasted from old controller, refactor
        }

/// <summary>
/// Add a new file
/// </summary>
/// <returns><see cref="JsonResult"/> for BlueImpUploadPlugin</returns>
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            IFormCollection form = await this.Request.ReadFormAsync();
            ILogger log = Log.ForContext("User", this.User.Identity.Name);
            log.Information("Starting file upload processing, number of files attached: {@filesAttached}", form.Files.Count);

            List<IReturnData> files = new List<IReturnData>();
            foreach (var file in form.Files)
            {
                var fileresult = new ViewDataUploadFilesResult();

                try
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    fileresult.name = fileName;
                    fileresult.size = file.Length;

                    var filePath = Path.Combine(this.fileUploadPath, fileName);
                    Directory.CreateDirectory(filePath);

                    var savefile = file.SaveAsAsync(filePath);

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

    var data = new ReturnData { files = files };
    log.Information(
        "Completed file upload processing, processed {@filesProcessed} out of {@filesAttached} files",
        data.files.Count,
        form.Files.Count);
            log.Verbose("Response {@fileData}", data.files);
            return this.Json(data);
        }

        /// <summary>
        /// Updates a file record
        /// </summary>
        /// <param name="id">The id of the file in database</param>
        /// <returns><see cref="NotImplementedException"/></returns>
        [HttpPut("{id}")]
        public IActionResult Put(string id)
        {
            return this.HttpBadRequest("Not implemented");
        }

        /// <summary>
        /// Deletes a file from database and disk
        /// </summary>
        /// <param name="id">ID of file in database</param>
        /// <returns><see cref="JsonResult"/> for BlueImpFileUploadPlugin</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.HttpUnauthorized();
            }

            var item = this.Files.Find(id);
            if (item == null)
            {
                return this.HttpNotFound("File not found in database");
            }

            var file = new FileInfo(item.Path);
            if (!file.Exists)
            {
                return this.HttpNotFound("File found in database, but not on disk");
            }

            file.DeleteAsync();
            this.Files.Remove(item.Key);

            ////ILogger log = Log.ForContext("User", User.Identity.Name);
            ////log.Information("Starting deletion of {@fileName}", name);
            ////ReturnDeleteData data = new ReturnDeleteData();
            ////data.files = new Dictionary<string, bool>();
            ////try
            ////{
            ////    FileInfo file = new FileInfo(Path.Combine(FileUploadPath, name));
            ////    if (file.Exists == true)
            ////        await file.DeleteAsync();
            ////    if (file.Exists == false)
            ////        throw new Exception("File not deleted!");
            ////    else
            ////        log.Information("File {@fileName} successfully deleted", file.Name);
            ////    data.files.Add(name, true);
            ////}
            ////catch (Exception ex)
            ////{
            ////    data.files.Add(name, false);
            ////    log.Warning("File deletion failed. {@Exception}", ex);
            ////}
            ////log.Information("Finished deletion of {@fileName}", name);
            return this.Json(string.Empty); // FIXME: return the right json
        }
    }
}