using Microsoft.AspNet.Mvc;
using Serilog;

namespace IntranetGJAK.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Log.ForContext("User", User.Identity.Name).Information("Requested Index");
            if (User.Identity.IsAuthenticated)
                return View();
            else
                return RedirectToAction("Login", "Account");
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}