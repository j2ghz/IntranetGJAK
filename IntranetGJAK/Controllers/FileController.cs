using IntranetGJAK.Models;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace IntranetGJAK.Controllers
{
    using System.IO;

    using Serilog;

    [Route("api/[controller]")]
    public class FileController : Controller
    {
        [FromServices]
        public IFileRepository Files { get; set; }

        // GET: api/values
        [HttpGet]
        public JsonResult Get()
        {
            Log.Information("Get triggerd");
            return Json(Files.GetAll());//TODO return the right format
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(string id)
        {
            if (!User.Identity.IsAuthenticated)
                return HttpUnauthorized();
            var item = Files.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            var file = new FileInfo(item.Path);
            return PhysicalFile(file.FullName, "application/octet-stream"); //copypasted from old controller, refactor
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}