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
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using IntranetGJAK.Models;
    using IntranetGJAK.Models.JSON.Blueimp_FileUpload;
    using IntranetGJAK.Tools;

    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Data.Entity;
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
        public FileController(IApplicationEnvironment hostingEnvironment)
        {
            this.log = Log.ForContext<FileController>();
            this.fileUploadPath = Path.Combine(hostingEnvironment.ApplicationBasePath, "Uploads");
            this.log.Verbose("File handler created with base path: {@basepath}", this.fileUploadPath);
        }

        [FromServices]
        public ApplicationDbContext Db { get; set; }

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
            var dbfiles = from file in this.Db.Files where file.Uploader == User.GetUserName() select file;
            foreach (var file in dbfiles)
            {
                files.files.Add(file.ToSerializeable());
                this.log.Verbose("Found {FileName} {Size}", file.Name, Format.Bytes(file.Size));
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

            var items = from record in this.Db.Files where record.Id == id select record;
            if (items.Count() != 1)
            {
                return this.HttpNotFound("File not found in database");
            }
            var item = items.First();

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
            this.log.Information("Request with {@filesAttached} file(s)", form.Files.Count);
            this.Response.StatusCode = 201;

            var files = new FilesData();
            foreach (var formFile in form.Files)
            {
                var file = new Models.File();
                try
                {
                    file.Id = System.Guid.NewGuid().ToString();
                    file.Path = Path.Combine(this.fileUploadPath, file.Id);

                    Directory.CreateDirectory(Path.GetPathRoot(file.Path));
                    var taskSave = formFile.SaveAsAsync(file.Path);

                    file.Name = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                    file.Size = formFile.Length;
                    file.Uploader = this.User.Identity.Name;
                    file.DateUploaded = DateTime.Now;
                    this.Db.Files.Add(file);

                    await taskSave;
                    this.Db.SaveChanges();
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
                    this.log.Warning("Processing error: {@Exception}", ex);
                    files.files.Add(error);
                    this.Response.StatusCode = 500;
                }
                finally
                {
                    if (file.Size != new FileInfo(file.Path).Length)
                    {
                        this.log.Error(
                            "File is a different size than advertised! {sizeAdvertised} != {Actualsize}",
                            file.Size,
                            new FileInfo(file.Path).Length);
                    }

                    this.log.Information(
                        "File '{name}' with a size of {size} processed",
                        file.Name,
                        Format.Bytes(file.Size));
                }
            }

            this.log.Information("{FileCount} file(s) processed", files.files.Count);
            this.log.Verbose("Response {@fileData}", files.files);
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

            var items = from record in this.Db.Files where record.Id == id select record;
            if (!items.Any())
            {
                this.log.Warning("File {id} not found in database", id);
                return this.HttpNotFound("File not found in database");
            }
            else if (items.Count() > 1)
            {
                this.log.Error("Multiple same id records returned from database {@records}", items);
            }
            var item = items.First();

            this.log.Information("File found: {FileName} {Size}", item.Name, Format.Bytes(item.Size));
            if (item.Path == null)
            {
                this.log.Error("Invalid Database Record {id}, removing", item.Id);
                this.Db.Files.Remove(item);
                removed = true;
            }
            else
            {
                var file = new FileInfo(item.Path);

                if (file.Exists)
                {
                    file.DeleteAsync();
                    this.Db.Files.Remove(item);
                    removed = true;
                }
                else
                {
                    this.log.Error("File found in database, but not on disk");
                }
            }
            this.Db.SaveChanges();
            DeletedData files = new DeletedData();

            files.files.Add(item.Name, removed);
            return this.Json(files);
        }
    }
}