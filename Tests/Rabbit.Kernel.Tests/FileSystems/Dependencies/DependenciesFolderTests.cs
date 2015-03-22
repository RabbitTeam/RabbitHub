using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.FileSystems.Application;
using Rabbit.Kernel.FileSystems.Dependencies;
using System;

namespace Rabbit.Kernel.Tests.FileSystems.Dependencies
{
    [TestClass]
    public sealed class DependenciesFolderTests : TestBase, IDisposable
    {
        #region Property

        public IDependenciesFolder DependenciesFolder { get; set; }

        public IApplicationFolder ApplicationFolder { get; set; }

        #endregion Property

        #region Test Method

        [TestMethod]
        public void StoreDescriptorsAndGetDescriptorTest()
        {
            Assert.IsNull(DependenciesFolder.GetDescriptor("Dependency1"));
            DependenciesFolder.StoreDescriptors(new[]
            {
                new DependencyDescriptor
                {
                    LoaderName = "Default",
                    Name = "Dependency1",
                    References = new []
                    {
                        new DependencyReferenceDescriptor
                        {
                            LoaderName = "Default",
                            Name = "Reference1",
                            VirtualPath = "~/Modules/Rabbit.Test/bin/2.dll"
                        }
                    },
                    VirtualPath = "~/Modules/Rabbit.Test/bin/1.dll"
                }
            });
            Assert.IsNotNull(DependenciesFolder.GetDescriptor("Dependency1"));
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