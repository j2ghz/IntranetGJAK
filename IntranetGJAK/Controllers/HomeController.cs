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
        public HomeController(IFileRepository files)
        {
            this.Files = files;
        }

        /// <summary>
        /// Gets or sets a list of all files from database
        /// </summary>
        [FromServices]
        private IFileRepository Files { get; set; }

        public IActionResult Index()
        {
            return View(this.Files.GetAll());
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
            return View();
        }
    }
}