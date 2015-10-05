using Backload.Contracts.Context;
using Backload.Contracts.FileHandler;
using Backload.Helper;
using System.Web;

namespace $rootnamespace$.aspnet
{
    public class FileHandler : IHttpHandler 
    {
        /// <summary>
        /// File handler demo for classic Asp.Net. 
        /// To access it in an Javascript ajax request use: <code>var url = "/aspnet/FileHandler.ashx";</code>.
        /// </summary>
        /// <remarks>
        /// NOTE. Edit the web.config file to allow the DELETE method in the system.webServer.handlers section
        /// </remarks>
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // Wrap the request into a HttpRequestBase type
                HttpRequestBase request = new HttpRequestWrapper(context.Request);


                // Create and initialize the handler
                IFileHandler handler = Backload.FileHandler.Create();
                handler.Init(request);


                // Call the execution pipeline and get the result
                IBackloadResult result = handler.Execute();


                // Write result to the response and flush
                ResultCreator.Write(context.Response, result);
                context.Response.Flush();

            }
            catch
            {
                context.Response.StatusCode = 500;
            }

        }

		
		/// <summary>
		/// Handler is not reusable
		/// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
