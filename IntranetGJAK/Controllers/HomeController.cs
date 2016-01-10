using Microsoft.AspNet.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetGJAK.Controllers
{
    using IntranetGJAK.Models;

    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        [FromServices]
        public ApplicationDbContext DbContext { get; set; }

        public IActionResult Index()
        {
            return this.View(this.DbContext.Files);
        }

        public IActionResult Upload()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return this.RedirectToAction("Login", "Account");
            }
        }
    

        public IActionResult Error()
        {
            return this.View();
        }
    }
}