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
    using System.Runtime.InteropServices;
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

        private readonly ILogger log;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">
        /// The hosting environment.
        /// </param>
        /// <param name="files">A list of files</param>
        public FileController(IApplicationEnvironment hostingEnvironment, IFileRepository files)
        {
            this.log = Log.ForContext<FileController>();
            this.Files = files;
            this.fileUploadPath = Path.Combine(hostingEnvironment.ApplicationBasePath, "Uploads");
            log.Verbose("File handler created with base path: {@basepath}", this.fileUploadPath);
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
            this.log.Information("Listing files:");
            var files = new FilesData();
            foreach (var file in this.Files.GetAll())
            {
                files.files.Add(file.ToSerializeable());
                this.log.Information("Found {FileName} {Size}", file.Name, Format.Bytes(file.Size));
            }
            this.log.Information("Found {FileCount} file(s)", files.files.Count);
            return this.Json(files);
        }

        /// <summary>
        /// Download a single file
        /// </summary>
        /// <param name="id">ID of file from database</param>
        /// <returns>File to be sent to client</returns>
        [ActionName("Index")]
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            ////if (!this.User.Identity.IsAuthenticated)
            ////{
            ////    return this.HttpUnauthorized();
            ////}

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

            return this.PhysicalFile(file.FullName, "application/octet-stream", item.Name); // copypasted from old controller, refactor
        }

        /// <summary>
        /// Add a new file
        /// </summary>
        /// <returns><see cref="JsonResult"/> for BlueImpUploadPlugin</returns>
        [ActionName("Index")]
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var form = await this.Request.ReadFormAsync();
            log.Information("Request with {@filesAttached} file(s)", form.Files.Count);
            this.Response.StatusCode = 201;

            var files = new FilesData();
            foreach (var formFile in form.Files)
            {
                var file = new Models.File();
                try
                {
                    file.Key = System.Guid.NewGuid().ToString();
                    file.Path = Path.Combine(this.fileUploadPath, file.Key);

                    Directory.CreateDirectory(Path.GetPathRoot(file.Path));
                    var taskSave = formFile.SaveAsAsync(file.Path);

                    file.Name = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                    file.Size = formFile.Length;
                    this.Files.Add(file);

                    await taskSave;
                    files.files.Add(file.ToSerializeable());
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
                    this.Response.StatusCode = 500;
                }
                finally
                {
                    log.Information("File '{name}' with a size of {size} processed.", file.Name, Format.Bytes(file.Size));
                }
            }

            log.Information("{FileCount} file(s) processed", files.files.Count);
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
            this.log.Information("Removing {id}", id);
            bool removed = false;
            ////if (!this.User.Identity.IsAuthenticated)
            ////{
            ////    this.log.Information("User Unauthorized");
            ////    return this.HttpUnauthorized();
            ////}

            var item = this.Files.Find(id);
            if (item == null)
            {
                this.log.Warning("File {id} not found in database", id);
                return this.HttpNotFound("File not found in database");
            }

            this.log.Information("File found: {FileName} {Size}", item.Name, Format.Bytes(item.Size));
            if (item.Path == null)
            {
                this.log.Error("Invalid Database Record {id}, removing", item.Key);
                this.Files.Remove(item.Key);
                removed = true;
            }
            else
            {
                var file = new FileInfo(item.Path);

                if (file.Exists)
                {
                    file.DeleteAsync();
                    this.Files.Remove(item.Key);
                    removed = true;
                }
                else
                {
                    this.log.Error("File found in database, but not on disk");
                }
            }
            DeletedData files = new DeletedData();
            
            files.files.Add(item.Key, removed);
            return this.Json(files);
        }
    }
}