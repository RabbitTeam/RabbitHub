using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.Environment.Assemblies.Models;
using Rabbit.Kernel.FileSystems.Application;
using Rabbit.Kernel.FileSystems.Dependencies;
using System;
using System.IO;

namespace Rabbit.Kernel.Tests.FileSystems.Dependencies
{
    [TestClass]
    public sealed class AssemblyProbingFolderTests : TestBase, IDisposable
    {
        #region Property

        public IAssemblyProbingFolder AssemblyProbingFolder { get; set; }

        public IApplicationFolder ApplicationFolder { get; set; }

        #endregion Property

        #region Constructor

        public AssemblyProbingFolderTests()
        {
            AssemblyProbingFolder.StoreAssembly(new AssemblyDescriptor("Rabbit.Kernel"), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rabbit.Kernel.dll"));
        }

        #endregion Constructor

        #region Test Method

        [TestMethod]
        public void AssemblyExistsTest()
        {
            Action<string, bool> test =
                (name, b) =>
                {
                    var result = AssemblyProbingFolder.AssemblyExists(new AssemblyDescriptor(name));
                    if (b)
                        Assert.IsTrue(result);
                    else
                        Assert.IsFalse(result);
                };

            test("Rabbit.Kernel1", false);
            test("Rabbit.Kernel", true);
        }

        [TestMethod]
        public void GetAssemblyDateTimeUtcTest()
        {
            Assert.IsNull(AssemblyProbingFolder.GetAssemblyDateTimeUtc(new AssemblyDescriptor("Rabbit.Kernel1")));

            Assert.IsNotNull(AssemblyProbingFolder.GetAssemblyDateTimeUtc(new AssemblyDescriptor("Rabbit.Kernel")));
        }

        [TestMethod]
        public void GetAssemblyVirtualPathTest()
        {
            Assert.AreEqual("~/App_Data/Dependencies/Rabbit.Kernel.dll", AssemblyProbingFolder.GetAssemblyVirtualPath(new AssemblyDescriptor("Rabbit.Kernel")));
        }

        [TestMethod]
        public void LoadAssemblyTest()
        {
            Assert.IsNull(AssemblyProbingFolder.LoadAssembly(new AssemblyDescriptor("Rabbit.Kernel1")));
            Assert.IsNotNull(AssemblyProbingFolder.LoadAssembly(new AssemblyDescriptor("Rabbit.Kernel")));
        }

        [TestMethod]
        public void StoreAndDeleteAssemblyTest()
        {
            var descriptor = new AssemblyDescriptor("Rabbit.Kernel.Tests");

            if (AssemblyProbingFolder.AssemblyExists(descriptor))
                AssemblyProbingFolder.DeleteAssembly(descriptor);

            AssemblyProbingFolder.StoreAssembly(descriptor, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, descriptor.Name + ".dll"));
            Assert.IsTrue(AssemblyProbingFolder.AssemblyExists(descriptor));
            AssemblyProbingFolder.DeleteAssembly(descriptor);
            Assert.IsFalse(AssemblyProbingFolder.AssemblyExists(descriptor));
        }

        [TestMethod]
        public void StoreAssemblyByModuleNameAndDeleteAssemblyByModuleNameTest()
        {
            AssemblyProbingFolder.DeleteAssembly("Rabbit.Test");

            Assert.IsFalse(AssemblyProbingFolder.AssemblyExists(new AssemblyDescriptor("Rabbit.Test")));

            AssemblyProbingFolder.StoreAssembly("Rabbit.Test");

            Assert.IsTrue(AssemblyProbingFolder.AssemblyExists(new AssemblyDescriptor("Rabbit.Test")));

            AssemblyProbingFolder.DeleteAssembly("Rabbit.Test");

            Assert.IsFalse(AssemblyProbingFolder.AssemblyExists(new AssemblyDescriptor("Rabbit.Test")));
        }

        #endregion Test Method

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            try { ApplicationFolder.DeleteDirectory("~/App_Data"); }
            catch { }
        }

        #endregion Implementation of IDisposable
    }
}