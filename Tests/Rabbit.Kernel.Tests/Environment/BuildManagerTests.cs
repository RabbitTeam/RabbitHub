using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.Environment;
using System;
using System.Linq;

namespace Rabbit.Kernel.Tests.Environment
{
    [TestClass]
    public sealed class BuildManagerTests : TestBase
    {
        public IBuildManager BuildManager { get; set; }

        [TestMethod]
        public void GetReferencedAssembliesTest()
        {
            Assert.IsTrue(BuildManager.GetReferencedAssemblies().Any());
        }

        [TestMethod]
        public void HasReferencedAssemblyTest()
        {
            Assert.IsTrue(BuildManager.HasReferencedAssembly("Rabbit.Kernel"));
            Assert.IsFalse(BuildManager.HasReferencedAssembly(Guid.NewGuid().ToString("n")));
        }

        [TestMethod]
        public void GetReferencedAssemblyTest()
        {
            Assert.IsNotNull(BuildManager.GetReferencedAssembly("Rabbit.Kernel"));
            Assert.IsNull(BuildManager.GetReferencedAssembly(Guid.NewGuid().ToString("N")));
        }
    }
}