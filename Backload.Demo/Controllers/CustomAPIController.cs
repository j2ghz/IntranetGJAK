using Backload.Contracts.Context;
using Backload.Contracts.FileHandler;
using Backload.Contracts.Status;
using Backload.Helper;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Backload.Demo.Controllers
{

    /// <summary>
    /// Custom controller for the events demo  Note: events must be enabled in the config.
    /// </summary>
    public class CustomAPIController : Controller
    {
        /// <summary>
        /// The Backload file handler. 
        /// To access it in an Javascript ajax request use: <code>var url = "/Backload/FileHandler/";</code>.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get|HttpVerbs.Post|HttpVerbs.Delete)]
        public async Task<ActionResult> FileHandler()
        {
            IBackloadResult result = null;
            IFileStatus status = null;
            try
            {
                // Create and initialize the handler
                IFileHandler handler = Backload.FileHandler.Create();
                handler.Init(HttpContext.Request);


                // This demo calls high level API methods. 
                // Http methhod related API methods are in handler.Services.[HttpMethod].
                // Low level API methods are in handler.Services.Core
                if (handler.Context.HttpMethod == "GET")
                    status = await handler.Services.GET.Execute();
                else if (handler.Context.HttpMethod == "POST")
                    status = await handler.Services.POST.Execute();
                else if (handler.Context.HttpMethod == "DELETE")
                    status = await handler.Services.DELETE.Execute();


                // Create a client plugin specific result. 
                // In this example we could simply call CreateResult(), because handler.FilesStatus also has the IFileStatus object
                result = handler.Services.Core.CreatePluginResult(status, Backload.Contracts.Context.Config.PluginType.JQueryFileUpload);


                // Helper to create an ActionResult object from the IBackloadResult instance
                return ResultCreator.Create(result);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

    }
}
