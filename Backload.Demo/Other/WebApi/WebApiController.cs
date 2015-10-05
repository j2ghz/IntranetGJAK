using Backload.Contracts.Context;
using Backload.Contracts.FileHandler;
using Backload.Contracts.Status;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Backload.Demo.WebApi
{
    public class FileHandlerController : ApiController
    {
        // GET api/<controller>
        public async Task<IClientPluginResultCore> Get()
        {
            try
            {
                IFileHandler handler = GetHandler();

                // Call the GET execution method and get the result
                IFileStatus status = await handler.Services.GET.Execute();

                // Create plugin specific result
                IFileStatusResult result = handler.Services.Core.CreatePluginResult();

                // return Json
                return result.ClientStatus;
            }
            catch
            {
                HttpContext.Current.Response.StatusCode = 500;
            }
            return null;
        }


        
        // GET api/<controller>?fileName=[name]  
        public async Task<HttpResponseMessage> Get(string fileName)
        {
            try
            {
                IFileHandler handler = GetHandler();

                // Call the GET execution method and get the result
                IFileStatus status = await handler.Services.GET.Execute();

                // Create plugin specific result
                IFileDataResult result = handler.Services.Core.CreateFileResult();

                // FIle data (bytes) requested
                HttpResponseMessage data = new HttpResponseMessage(HttpStatusCode.OK);
                data.Content = new ByteArrayContent(result.FileData);
                data.Content.Headers.ContentType = new MediaTypeHeaderValue(result.ContentType);

                return data;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }



        // POST api/<controller>
        public async Task<IClientPluginResultCore> Post()
        {
            try
            {
                IFileHandler handler = GetHandler();

                // Call the POST execution method and get the result
                IFileStatus status = await handler.Services.POST.Execute();

                // Create plugin specific result
                IFileStatusResult result = handler.Services.Core.CreatePluginResult();

                // return Json
                return result.ClientStatus;
            }
            catch
            {
                HttpContext.Current.Response.StatusCode = 500;
            }
            return null;

        }



        // DELETE api/<controller>?fileName=[name]
        public async Task<IClientPluginResultCore> Delete(string fileName)
        {
            try
            {
                IFileHandler handler = GetHandler();

                // Call the DELETE execution method and get the result
                IFileStatus status = await handler.Services.DELETE.Execute();

                // Create plugin specific result
                IFileStatusResult result = handler.Services.Core.CreatePluginResult();

                // return Json
                return result.ClientStatus;
            }
            catch
            {
                HttpContext.Current.Response.StatusCode = 500;
            }
            return null;

        }



        // Creates and inits an IFileHandler instance
        private static IFileHandler GetHandler()
        {
            // Wrap the request into a HttpRequestBase type
            HttpRequestBase request = new HttpRequestWrapper(HttpContext.Current.Request);


            // Create and initialize the handler
            IFileHandler handler = Backload.FileHandler.Create();
            handler.Init(request);
            return handler;
        }

    }
}