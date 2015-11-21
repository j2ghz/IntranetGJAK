using IntranetGJAK.Controllers;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Runtime.Versioning;
using Xunit;

namespace UnitTests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Home
    {
        [Fact]
        public void ControllerNotNull()
        {
            Assert.NotNull(new HomeController());
        }

        [Fact]
        public void IndexNotLoggedInRedirects()
        {
            var home = new HomeController();
            var result = home.Index();
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            RedirectToActionResult redirect = (RedirectToActionResult)result;
            Assert.Equal("Account", redirect.ControllerName);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal(false, redirect.Permanent);
            Assert.Empty(redirect.RouteValues);
        }
    }

    public class UploadController
    {
        [Fact]
        public void ControllerNotNull()
        {
            Assert.NotNull(new Files(new appenv()));
        }
    }

    public class appenv : IApplicationEnvironment
    {
        string IApplicationEnvironment.ApplicationBasePath
        {
            get
            {
                return "";
            }
        }

        string IApplicationEnvironment.ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IApplicationEnvironment.ApplicationVersion
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IApplicationEnvironment.Configuration
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        FrameworkName IApplicationEnvironment.RuntimeFramework
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        object IApplicationEnvironment.GetData(string name)
        {
            throw new NotImplementedException();
        }

        void IApplicationEnvironment.SetData(string name, object value)
        {
            throw new NotImplementedException();
        }
    }
}