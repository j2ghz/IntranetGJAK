using Backload.Contracts.Context;
using System;
using System.Web;
using System.Web.Mvc;

namespace Backload.Helper
{
    /// <summary>
    /// This helper class creates the result of type ActionResult from the IBackloadResult object
    /// </summary>
    public partial class ResultCreator
    {

        /// <summary>
        /// Returns an ActionResult depending on the requested type (Json or file data)
        /// </summary>
        /// <param name="result">An IBackloadResult instance</param>
        /// <returns>An ActionResult instance</returns>
        public static ActionResult Create(IBackloadResult result)
        {
            // RequestType.Default: Json output has been requested (default). 
            // Otherwise a file or a thumbnail (bytes) will be returned.
            if (result.RequestType == RequestType.Default)
                return Create((IFileStatusResult)result);
            else if ((result.RequestType == RequestType.File) || (result.RequestType == RequestType.Thumbnail))
                return Create((IFileDataResult)result);
            else
                return new EmptyResult();
        }



        /// <summary>
        /// Creates the Json output for the files handled in this request
        /// </summary>
        /// <param name="result">A IFileStatusResult object with client plugin specfic data.</param>
        /// <returns>JsonResult instance or a http HttpStatusCodeResult to send an http status</returns>
        public static ActionResult Create(IFileStatusResult result)
        {
            // Create Json result from the returned client plugin specific file metadata.
            if ((result.ClientStatus != null) && (result.Exception == null))
                return new JsonResult()
                {
                    Data = result.ClientStatus,
                    ContentType = result.ContentType,
                    ContentEncoding = System.Text.Encoding.UTF8,
                    MaxJsonLength = Int32.MaxValue,
                    RecursionLimit = null,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };


            // A HttpStatusCodeResult result is returned on errors or if files not have been modified (304)
            return new HttpStatusCodeResult(result.HttpStatusCode);
        }



        /// <summary>
        /// Handles file data (bytes) if Backload is configured to handle file requests (e.g. thumbsUrlPattern) 
        /// </summary>
        /// <param name="result">A IFileDataResult object with file data (bytes).</param>
        /// <returns>FileContentResult instance or a http HttpStatusCodeResult to send an http status</returns>
        public static ActionResult Create(IFileDataResult result)
        {
            // Create a new FileContentResult from the returned file data.
            if ((result.FileData != null) && (result.Exception == null))
                return new FileContentResult(result.FileData, result.ContentType);


            // A HttpStatusCodeResult result is returned on errors or if files not have been modified (304)
            return new HttpStatusCodeResult(result.HttpStatusCode);
        }

    }
}