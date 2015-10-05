using Backload.Contracts.Context;
using Backload.Contracts.FileHandler;
using Backload.Contracts.Status;
using $rootnamespace$.Models;
using Backload.Helper;
using Backload.Status;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace $rootnamespace$.Controllers
{

    /// <summary>
    /// A custom database controller used in the database storage demo
    /// </summary>
    /// <remarks>
    /// IMPORTANT NOTE:
    /// Because we use the same configuration file for all demos, this database demo implements a workaround for the missing
    /// filesUrlPattern="{Backload}" setting in the configuration file. In your project configure filesUrlPattern="{Backload}"
    /// </remarks>
    public class CustomDatabaseController : Controller
    {
        /// <summary>
        /// Custom database handler. 
        /// To access it in an Javascript ajax request use: <code>var url = "/CustomDatabase/DataHandler";</code>.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post | HttpVerbs.Put | HttpVerbs.Delete | HttpVerbs.Options)]
        public async Task<ActionResult> DataHandler()
        {
            try
            {
                // Create and initialize the handler
                IFileHandler handler = Backload.FileHandler.Create();
                handler.Init(HttpContext.Request);

                // Call the appropriate request handlers
                if (handler.Context.HttpMethod == "POST")
                    return await PostHandler(handler);

                else if (handler.Context.HttpMethod == "GET")
                    // Usually, we use IBackloadContext.RequestType to decide if this is a standard GET request with Json response or a file data 
                    // response. But all demos share the same configuration and we have to configure filesUrlPatter="{Backload}".
                    // So this is an alternative approach. In your code use filesUrlPatter="{Backload}".
                    // if (handler.Context.RequestType == RequestType.Default) 
                    if (handler.RequestValues.BackloadValues.FileName == string.Empty)
                        return GetHandler(handler);
                    else
                        return GetFileHandler(handler);

                else if (handler.Context.HttpMethod == "DELETE")
                    return await DeleteHandler(handler);


                // other http methods may also be handled
                return new EmptyResult();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }



        // Executes GET requsts for all files
        private static ActionResult GetHandler(IFileHandler handler)
        {
            IFileStatus status = new FileStatus();
            using (FilesModel db = new FilesModel())
            {
                foreach (var row in db.Files)
                {
                    string url = handler.Context.Request.Url.OriginalString + "?fileName=" + row.Id.ToString();

                    IFileStatusItem file = new FileStatusItem()
                    {
                        ContentType = row.Type,
                        DeleteType = "DELETE",
                        FileName = row.Name,
                        FileSize = row.Size,
                        OriginalName = row.Original,
                        Progress = "100",
                        Success = true,
                        ThumbnailUrl = row.Preview,

                        // Set an identifier for GET and DELETE requests
                        DeleteUrl = url,
                        FileUrl = url
                    };

                    status.Files.Add(file);
                }
            }

            handler.FileStatus = status;

            // Create client plugin specific result and return an ActionResult
            IBackloadResult result = handler.Services.Core.CreatePluginResult();
            return ResultCreator.Create((IFileStatusResult)result);
        }




        // Executes GET requsts for a specific file (e.g. user has clicked on a preview)
        private static ActionResult GetFileHandler(IFileHandler handler)
        {
            // Get the requested file name
            string fileName = handler.RequestValues.BackloadValues.FileName;
            Guid id = Guid.Parse(fileName);

            string contentType = string.Empty;
            byte[] data = null;

            using (FilesModel db = new FilesModel())
            {
                File row = db.Files.Find(id);

                contentType = row.Type;
                data = row.Data;
            }

            // return the raw file data (bytes)
            return new FileContentResult(data, contentType);
        }




        // Executes DELETE requsts
        private async Task<ActionResult> DeleteHandler(IFileHandler handler)
        {
            // Get the file to delete
            handler.FileStatus = handler.Services.DELETE.GetRequestStatus();
            IFileStatusItem file = handler.FileStatus.Files[0];

            using (FilesModel db = new FilesModel())
            {
                Guid id = Guid.Parse(file.FileName);

                File f = db.Files.Find(id);
                db.Files.Remove(f);
                await db.SaveChangesAsync();
            }

            // Create client plugin specific result and return an ActionResult
            IBackloadResult result = handler.Services.Core.CreatePluginResult();
            return ResultCreator.Create((IFileStatusResult)result);
        }




        // Executes POST requsts
        private async Task<ActionResult> PostHandler(IFileHandler handler)
        {
            // Get the posted file with meta data from the request
            handler.FileStatus = await handler.Services.POST.GetPostedFiles();
            if (handler.FileStatus != null)
            {
                Guid id = Guid.NewGuid();
                IFileStatusItem file = handler.FileStatus.Files[0];


                // Read the file data (bytes)
                byte[] data = null;
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream((int)file.FileSize))
                {
                    await file.FileStream.CopyToAsync(stream);
                    data = stream.ToArray();
                }


                // Create the thumbnail
                await handler.Services.POST.CreateThumbnail(file);

                // Create a base64 encoded data url for the thumbnail we can return in Json
                await handler.Services.Core.CreateThumbnailDataUrls();


                // This will change the url query parameter fileName to the id of the 
                // new table row, so we can identify the file in a DELETE and GET request
                file.FileUrl = file.DeleteUrl.Replace(file.FileName, id.ToString());
                file.DeleteUrl = file.FileUrl;


                // Store to db
                using (FilesModel db = new FilesModel())
                {
                    // File entity represents a table row
                    File row = new File()
                    {
                        Id = id,
                        Data = data,
                        Name = file.FileName,
                        Original = file.OriginalName,
                        Size = file.FileSize,
                        Type = file.ContentType,
                        UploadTime = DateTime.Now,
                        Preview = file.ThumbnailUrl
                    };

                    File f = db.Files.Add(row);
                    await db.SaveChangesAsync();
                };

            }


            // Create client plugin specific result and return an ActionResult
            IBackloadResult result = handler.Services.Core.CreatePluginResult();
            return ResultCreator.Create((IFileStatusResult)result);
        }

    }
}
