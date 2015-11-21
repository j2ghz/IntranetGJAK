using IntranetGJAK.Controllers;
using LightMock;
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
    }

    public class UploadController
    {
        [Fact]
        public void ControllerNotNull()
        {
            var mock = new LightMock.MockContext<Microsoft.Extensions.PlatformAbstractions.IApplicationEnvironment>();
            var env = new appenv(mock);
            mock.Arrange(f => f.ApplicationBasePath).Returns("");
            Assert.NotNull(new Files(env));
        }
    }

    public class appenv : IApplicationEnvironment
    {
        private readonly IInvocationContext<IApplicationEnvironment> context;

        public appenv(IInvocationContext<IApplicationEnvironment> context)
        {
            this.context = context;
        }

        string IApplicationEnvironment.ApplicationBasePath
        {
            get
            {
                return context.Invoke(f => f.ApplicationBasePath);
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