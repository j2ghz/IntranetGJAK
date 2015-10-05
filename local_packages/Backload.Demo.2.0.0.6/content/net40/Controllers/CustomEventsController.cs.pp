using Backload.Contracts.Context;
using Backload.Contracts.FileHandler;
using Backload.Contracts.Status;
using Backload.Helper;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace $rootnamespace$.Controllers
{

    /// <summary>
    /// Custom controller for the events demo  Note: events must be enabled in the config.
    /// </summary>
    public class CustomEventsController : Controller
    {
        /// <summary>
        /// The Backload file handler. 
        /// To access it in an Javascript ajax request use: <code>var url = "/Backload/FileHandler/";</code>.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get|HttpVerbs.Post|HttpVerbs.Put|HttpVerbs.Delete|HttpVerbs.Options)]
        public ActionResult FileHandler()
        {
            try
            {
                // Create and initialize the handler
                IFileHandler handler = Backload.FileHandler.Create();


                // Attach event handlers to events
                handler.Events.IncomingRequestStarted += Events_IncomingRequestStarted;
                handler.Events.GetFilesRequestStarted += Events_GetFilesRequestStarted;
                handler.Events.GetFilesRequestFinished += Events_GetFilesRequestFinished;
                

                // Init Backloads execution environment and execute the request
                handler.Init(HttpContext.Request);
                IBackloadResult result = handler.Execute();


                // Helper to create an ActionResult object from the IBackloadResult instance
                return ResultCreator.Create(result);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        void Events_IncomingRequestStarted(IFileHandler sender, Backload.Contracts.Eventing.IIncomingRequestEventArgs e)
        {
            // Backload environment is ready
        }


        void Events_GetFilesRequestStarted(IFileHandler sender, Backload.Contracts.Eventing.IGetFilesRequestEventArgs e)
        {
            // Backload component has started the internal GET handler method. 
            string searchPath = e.Param.SearchPath;
        }


        void Events_GetFilesRequestFinished(IFileHandler sender, Backload.Contracts.Eventing.IGetFilesRequestEventArgs e)
        {
            // Backload component has finished the internal GET handler method. 
            // Results can be found in e.Param.FileStatus or sender.FileStatus

            IFileStatus status = e.Param.FileStatus;
        }

    }
}
