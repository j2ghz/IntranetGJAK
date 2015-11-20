using IntranetGJAK.Controllers;
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