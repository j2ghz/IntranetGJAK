// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileController.cs" company="Jozef Holly">
//   Copyright
// </copyright>
// <summary>
//   Controller for WebAPI used for uploading and downloading files
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IntranetGJAK.Models;
using IntranetGJAK.Models.JSON.Blueimp_FileUpload;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Net.Http.Headers;
using Serilog;
using File = IntranetGJAK.Models.File;

namespace IntranetGJAK.Controllers
{
    // For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
    
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
            log = Log.ForContext<FileController>();
            fileUploadPath = Path.Combine(hostingEnvironment.ApplicationBasePath, "Uploads");
            log.Verbose("File handler created with base path: {@basepath}", fileUploadPath);
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
            log.Information("Listing files:");
            var files = new FilesData();
            var dbfiles = from file in Db.Files where file.Uploader == User.GetUserName() select file;
            foreach (var file in dbfiles)
            {
                files.files.Add(file.ToSerializeable());
                log.Verbose("Found {FileName} {Size}", file.Name, Format.Bytes(file.Size));
            }
            log.Information("Found {FileCount} file(s)", files.files.Count);
            return Json(files);
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

            var items = from record in Db.Files where record.Id == id select record;
            if (items.Count() != 1)
            {
                return HttpNotFound("File not found in database");
            }
            var item = items.First();

            var file = new FileInfo(item.Path);
            if (!file.Exists)
            {
                return HttpNotFound("File found in database, but not on disk");
            }

            return PhysicalFile(file.FullName, "application/octet-stream", item.Name); // copypasted from old controller, refactor
        }

        /// <summary>
        /// Add a new file
        /// </summary>
        /// <returns><see cref="JsonResult"/> for BlueImpUploadPlugin</returns>
        [ActionName("Index")]
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var form = await Request.ReadFormAsync();
            log.Information("Request with {@filesAttached} file(s)", form.Files.Count);
            Response.StatusCode = 201;

            var files = new FilesData();
            foreach (var formFile in form.Files)
            {
                var file = new File();
                try
                {
                    file.Id = Guid.NewGuid().ToString();
                    file.Path = Path.Combine(fileUploadPath, file.Id);

                    Directory.CreateDirectory(Path.GetPathRoot(file.Path));
                    var taskSave = formFile.SaveAsAsync(file.Path);

                    file.Name = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');
                    file.Size = formFile.Length;
                    file.Uploader = User.Identity.Name;
                    file.DateUploaded = DateTime.Now;
                    Db.Files.Add(file);

                    await taskSave;
                    Db.SaveChanges();
                    files.files.Add(file.ToSerializeable());
                }
                catch (Exception ex)
                {
                    var error = new UploadFailed
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
                    if (file.Size != new FileInfo(file.Path).Length)
                    {
                        log.Error(
                            "File is a different size than advertised! {sizeAdvertised} != {Actualsize}",
                            file.Size,
                            new FileInfo(file.Path).Length);
                    }

                    log.Information(
                        "File '{name}' with a size of {size} processed",
                        file.Name,
                        Format.Bytes(file.Size));
                }
            }

            log.Information("{FileCount} file(s) processed", files.files.Count);
            log.Verbose("Response {@fileData}", files.files);
            return Json(files);
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
            return HttpBadRequest("Not implemented");
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
            log.Information("Removing {id}", id);
            bool removed = false;
            ////if (!this.User.Identity.IsAuthenticated)
            ////{
            ////    this.log.Information("User Unauthorized");
            ////    return this.HttpUnauthorized();
            ////}

            var items = from record in Db.Files where record.Id == id select record;
            if (!items.Any())
            {
                log.Warning("File {id} not found in database", id);
                return HttpNotFound("File not found in database");
            }
            if (items.Count() > 1)
            {
                log.Error("Multiple same id records returned from database {@records}", items);
            }
            var item = items.First();

            log.Information("File found: {FileName} {Size}", item.Name, Format.Bytes(item.Size));
            if (item.Path == null)
            {
                log.Error("Invalid Database Record {id}, removing", item.Id);
                Db.Files.Remove(item);
                removed = true;
            }
            else
            {
                var file = new FileInfo(item.Path);

                if (file.Exists)
                {
                    file.DeleteAsync();
                    Db.Files.Remove(item);
                    removed = true;
                }
                else
                {
                    log.Error("File found in database, but not on disk");
                }
            }
            Db.SaveChanges();
            DeletedData files = new DeletedData();

            files.files.Add(item.Name, removed);
            return Json(files);
        }
    }
}