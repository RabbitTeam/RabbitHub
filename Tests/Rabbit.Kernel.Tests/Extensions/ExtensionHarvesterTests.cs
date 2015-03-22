using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.Extensions.Folders;
using System.Linq;

namespace Rabbit.Kernel.Tests.Extensions
{
    [TestClass]
    public sealed class ExtensionHarvesterTests : TestBase
    {
        public IExtensionHarvester ExtensionHarvester { get; set; }

        //        public ILifetimeScope LifetimeScope { get; set; }

        [TestMethod]
        public void HarvestExtensionsTest()
        {
            var extensions = ExtensionHarvester.HarvestExtensions(new[] { "~/Modules" }, "Module", "Module.txt", false);

            Assert.IsTrue(extensions.Any());
        }
    }
}