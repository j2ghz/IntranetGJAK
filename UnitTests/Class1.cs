using IntranetGJAK.Controllers;
using Microsoft.AspNet.Mvc;
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
            Assert.True(true);
        }

        [Fact]
        public void Error()
        {
            var controller = new HomeController();
            Assert.NotNull(controller);
            var result = controller.Error();
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            ViewResult view = (ViewResult)result;
            Assert.NotNull(view);
            Assert.Equal("~/Views/Shared/Error.cshtml", view.ViewName);
        }
    }

    public class UploadController
    {
        [Fact]
        public void ControllerNotNull()
        {
            Assert.False(true);
        }
    }
}