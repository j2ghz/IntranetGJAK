using Backload.Demo.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

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

        [TestMethod]
        public void FakeUploadFiles()
        {
            Assert.Inconclusive("Test not ready");
            throw new NotImplementedException("Test not ready");
            //We'll need mocks (fake) of Context, Request and a fake PostedFile
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            var postedfile = new Mock<HttpPostedFileBase>();

            //Someone is going to ask for Request.File and we'll need a mock (fake) of that.
            var postedfilesKeyCollection = new Mock<HttpFileCollectionBase>();
            var fakeFileKeys = new List<string>() { "file" };

            //OK, Mock Framework! Expect if someone asks for .Request, you should return the Mock!
            context.Expect(ctx => ctx.Request).Returns(request.Object);
            //OK, Mock Framework! Expect if someone asks for .Files, you should return the Mock with fake keys!
            request.Expect(req => req.Files).Returns(postedfilesKeyCollection.Object);

            //OK, Mock Framework! Expect if someone starts foreach'ing their way over .Files, give them the fake strings instead!
            postedfilesKeyCollection.Expect(keys => keys.GetEnumerator()).Returns(fakeFileKeys.GetEnumerator());

            //OK, Mock Framework! Expect if someone asks for file you give them the fake!
            postedfilesKeyCollection.Expect(keys => keys["file"]).Returns(postedfile.Object);

            //OK, Mock Framework! Give back these values when asked, and I will want to Verify that these things happened
            postedfile.Expect(f => f.ContentLength).Returns(8192).Verifiable();
            postedfile.Expect(f => f.FileName).Returns("foo.doc").Verifiable();

            //OK, Mock Framework! Someone is going to call SaveAs, but only once!
            postedfile.Expect(f => f.SaveAs(It.IsAny<string>())).AtMostOnce().Verifiable();

            Backload.Controllers.BackloadController controller = new Backload.Controllers.BackloadController();
            //Set the controller's context to the mock! (fake)
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            //DO IT!
            //await controller.FileHandler();

            //Now, go make sure that the Controller did its job
            //var uploadedResult = result.ViewData.Model as List<ViewDataUploadFilesResult>;

            //Assert.AreEqual(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "foo.doc"), uploadedResult[0].Name);
            //Assert.AreEqual(8192, uploadedResult[0].Length);

            postedfile.Verify();
        }
    }
}