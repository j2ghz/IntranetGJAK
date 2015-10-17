using Backload.Demo.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Web.Mvc;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void IndexRedirect()
        {
            HomeController controller = new HomeController();
            var result = controller.Index() as RedirectToRouteResult;
            Assert.IsTrue(result.RouteValues.ContainsKey("action"));
            Assert.AreEqual("BasicPlusUI", result.RouteValues["action"]);
        }

        [TestMethod]
        public void BasicPlusUI()
        {
            HomeController controller = new HomeController();
            var result = controller.BasicPlusUI() as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}