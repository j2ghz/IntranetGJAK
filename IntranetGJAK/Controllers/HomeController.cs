using Microsoft.AspNet.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntranetGJAK.Controllers
{
    using IntranetGJAK.Models;

    using Microsoft.AspNet.Authorization;

    [Authorize]
    public class HomeController : Controller
    {

        [FromServices]
        public ApplicationDbContext DbContext { get; set; }
        [Authorize(Roles = "Write, Read")]
        public IActionResult Index()
        {
            return this.View(this.DbContext.Files);
        }

        [Authorize(Roles = "Write")]
        public IActionResult Upload()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return this.View();
        }
    }
}