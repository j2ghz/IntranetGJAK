using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace $rootnamespace$.Controllers
{
    using System.IO;

    public class HomeController : Controller
    {

        // Start view
        public ActionResult Index()
        {
            return View();
        }


        #region jQuery File Upload Plugin

        // Basic theme (Bootstrap) 
        public ActionResult Basic()
        {
            return View();
        }  
  
        //Basic Plus theme (Bootstrap) 
        public ActionResult BasicPlus()
        {
            return View();
        }

        // Basic Plus UI theme (Bootstrap) 
        public ActionResult BasicPlusUI()
        {
            return View();
        }

        // AngularJS theme
        public ActionResult AngularJS()
        {
            return View();
        }

        // jQuery UI theme
        public ActionResult JQueryUI()
        {
            return View();
        }

#endregion



        #region PlUpload

        // Moxiecode PlUpload plugin simple demo
        public ActionResult PlUploadSimple()
        {
            return View();
        }

        // Moxiecode PlUpload plugin ui demo
        public ActionResult PlUploadUI()
        {
            return View();
        }

        #endregion



        #region Fine Uploader

        // Fine Uploader default demo
        public ActionResult FineUploaderDefault()
        {
            return View();
        }

        // Fine Uploader gallery demo
        public ActionResult FineUploaderGallery()
        {
            return View();
        }

        // Fine Uploader simple thumbnails demo
        public ActionResult FineUploaderSimple()
        {
            return View();
        }

        #endregion



        #region Custom controller

        // Custom controller with events demo
        public ActionResult CustomEvents()
        {
            return View();
        }

        // Custom controller with basic API method calls
        public ActionResult CustomAPI()
        {
            return View();
        }

        // Custom controller with database storage demo
        public ActionResult CustomDB()
        {
            return View();
        }

        #endregion

    }
}
