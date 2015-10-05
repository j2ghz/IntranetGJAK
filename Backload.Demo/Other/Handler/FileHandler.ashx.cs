using Backload.Contracts.Context;
using Backload.Contracts.FileHandler;
using Backload.Helper;
using System.Threading.Tasks;
using System.Web;

namespace Backload.Demo.Handler
{
    public class FileHandler : HttpTaskAsyncHandler
    {
        /// <summary>
        /// File handler demo for classic Asp.Net or HTML. 
        /// To access it in an Javascript ajax request use: <code>var url = "/Handler/FileHandler.ashx";</code>.
        /// </summary>
        /// <remarks>
        /// NOTE. Edit the web.config file to allow the DELETE method in the system.webServer.handlers section
        /// </remarks>
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            try
            {
                // Wrap the request into a HttpRequestBase type
                HttpRequestBase request = new HttpRequestWrapper(context.Request);


                // Create and initialize the handler
                IFileHandler handler = Backload.FileHandler.Create();
                handler.Init(request);


                // Call the execution pipeline and get the result
                IBackloadResult result = await handler.Execute();


                // Write result to the response and flush
                ResultCreator.Write(context.Response, result);
                context.Response.Flush();

            }
            catch
            {
                context.Response.StatusCode = 500;
            }

        }
    }
}