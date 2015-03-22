using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.FileSystems.Application;
using System;
using System.Linq;

namespace Rabbit.Kernel.Tests.Environment
{
    [TestClass]
    public sealed class HostTest : TestBase, IDisposable
    {
        #region Property

        public IHost Host { get; set; }

        public IExtensionLoaderCoordinator ExtensionLoaderCoordinator { get; set; }

        public IApplicationFolder ApplicationFolder { get; set; }

        public IShellSettingsManager ShellSettingsManager { get; set; }

        #endregion Property

        #region Constructor

        public HostTest()
        {
            ExtensionLoaderCoordinator.SetupExtensions();
        }

        #endregion Constructor

        #region Test Method

        [TestMethod]
        public void InitializeTest()
        {
            Host.Initialize();
            foreach (var setting in ShellSettingsManager.LoadSettings())
            {
                var context = Host.GetShellContext(setting);
                Assert.IsNotNull(context);
            }
        }

        private class TempDisposable : IDisposable
        {
            public static bool IsDispose;

            #region Implementation of IDisposable

            /// <summary>
            /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
            /// </summary>
            public void Dispose()
            {
                IsDispose = true;
            }

            #endregion Implementation of IDisposable
        }

        [TestMethod]
        public void ReloadExtensionsTest()
        {
            var contexts = ShellSettingsManager.LoadSettings().Select(i => Host.GetShellContext(i)).ToArray();
            contexts.First().Container.Disposer.AddInstanceForDisposal(new TempDisposable());
            Host.ReloadExtensions();
            Assert.IsTrue(TempDisposable.IsDispose);
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