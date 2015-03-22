using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.Environment;

namespace Rabbit.Kernel.Tests.Environment
{
    [TestClass]
    public sealed class HostEnvironmentTests : TestBase
    {
        #region Property

        public IHostEnvironment HostEnvironment { get; set; }

        #endregion Property

        #region Test Method

        [TestMethod]
        public void IsAssemblyLoadedTest()
        {
            Assert.IsTrue(HostEnvironment.IsAssemblyLoaded("system"));
            Assert.IsTrue(HostEnvironment.IsAssemblyLoaded(typeof(string).Assembly.FullName));
            Assert.IsTrue(HostEnvironment.IsAssemblyLoaded(typeof(string).Assembly.FullName.ToLower()));
        }

        #endregion Test Method
    }
}