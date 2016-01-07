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
    using IntranetGJAK.Models.JSON.Blueimp_FileUpload;
    using IntranetGJAK.Tools;

    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.Net.Http.Headers;

    using Serilog;

    /// <summary>
    /// Controller for WebAPI used for uploading and downloading files
    /// </summary>
    [Route("api/files")]
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
        [ActionName("Index")]
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
        [ActionName("Index")]
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
        [ActionName("Index")]
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            IFormCollection form = await this.Request.ReadFormAsync();
            ILogger log = Log.ForContext("User", this.User.Identity.Name);
            log.Information("Starting file upload processing, number of files attached: {@filesAttached}", form.Files.Count);

            var files = new FilesData();
            foreach (var formFile in form.Files)
            {
                var file = new Models.File();
                    var fileresult = new Models.JSON.Blueimp_FileUpload.UploadSucceeded();
                try
                {
                    var fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                    fileresult.name = fileName;
                    fileresult.size = formFile.Length;

                    var filePath = Path.Combine(this.fileUploadPath, fileName);
                    Directory.CreateDirectory(filePath);

                    var savefile = formFile.SaveAsAsync(filePath);

                    fileresult.url = "/api/files?id=" + fileName;
                    fileresult.thumbnailUrl = Thumbnails.GetThumbnail(fileName);
                    fileresult.deleteUrl = "/Files/Index/?name=" + fileName;
                    fileresult.deleteType = "DELETE";


                    await savefile;
                    files.files.Add(fileresult);
                    Response.StatusCode = 201;
                }
                catch (Exception ex)
                {
                    var error = new UploadFailed()
                    {
                        name = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"'),
                        size = formFile.Length,
                        error = ex.ToString()
                    };
                    log.Warning("Processing error: {@Exception}", ex);
                    files.files.Add(error);
                    Response.StatusCode = 500;
                }
                finally
                {
                    log.Information("Processed file: {@fileName} {@fileSize}", fileresult.name, Formatting.FormatBytes(fileresult.size));
                }
            }
            log.Information(
                "Completed file upload processing, processed {@filesProcessed} out of {@filesAttached} files",
                files.files.Count,
                form.Files.Count);
            log.Verbose("Response {@fileData}", files.files);
            return this.Json(files);
        }

        /// <summary>
        /// Updates a file record
        /// </summary>
        /// <param name="id">The id of the file in database</param>
        /// <returns><see cref="NotImplementedException"/></returns>
        [ActionName("Index")]
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
        [ActionName("Index")]
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

            DeletedData files = new DeletedData();


            return this.Json(files);
        }
    }
}