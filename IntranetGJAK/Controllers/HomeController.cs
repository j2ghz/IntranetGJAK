using Microsoft.AspNet.Mvc;
using Serilog;

namespace IntranetGJAK.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Log.Information("Requested Index");
            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}