
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntranetGJAK.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IntranetGJAK.Models;

namespace IntranetGJAK.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        ApplicationDbContext DbContext { get; set; }

        HomeController(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

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
