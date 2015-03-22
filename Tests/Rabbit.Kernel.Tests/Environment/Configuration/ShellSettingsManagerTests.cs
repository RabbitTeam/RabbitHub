using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.FileSystems.VirtualPath;
using System;
using System.Linq;

namespace Rabbit.Kernel.Tests.Environment.Configuration
{
    [TestClass]
    public sealed class ShellSettingsManagerTests : TestBase, IDisposable
    {
        #region Field

        /// <summary>
        /// 事件是否被执行。
        /// </summary>
        private static bool _eventIsRun;

        #endregion Field

        #region Property

        public IShellSettingsManager ShellSettingsManager { get; set; }

        public IVirtualPathProvider VirtualPathProvider { get; set; }

        #endregion Property

        #region Test Method

        [TestMethod]
        public void SaveSettingsAndLoadSettingsTest()
        {
            var settings = new[]
            {
                new ShellSettings {Name = "租户1"},
                new ShellSettings {Name = "租户2"},
                new ShellSettings {Name = "租户3"}
            };
            foreach (var setting in settings)
                ShellSettingsManager.SaveSettings(setting);

            var settingsList = ShellSettingsManager.LoadSettings().ToArray();

            Assert.IsTrue(settingsList.Length >= 3);
            foreach (var setting in settings)
            {
                Assert.IsTrue(settingsList.Any(i => i.Name == setting.Name));
            }

            Assert.IsTrue(_eventIsRun);
        }

        #endregion Test Method

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            try
            {
                VirtualPathProvider.DeleteDirectory("~/App_Data");
            }
            catch { }
        }

        #endregion Implementation of IDisposable

        #region Overrides of TestBase

        protected override void Register(ContainerBuilder builder)
        {
            builder.RegisterType<TempShellSettingsManagerEventHandler>().As<IShellSettingsManagerEventHandler>();
        }

        #endregion Overrides of TestBase

        #region Help Class

        internal sealed class TempShellSettingsManagerEventHandler : IShellSettingsManagerEventHandler
        {
            #region Implementation of IShellSettingsManagerEventHandler

            /// <summary>
            /// 外壳设置保存成功之后。
            /// </summary>
            /// <param name="settings">外壳设置信息。</param>
            public void Saved(ShellSettings settings)
            {
                _eventIsRun = true;
            }

            #endregion Implementation of IShellSettingsManagerEventHandler
        }

        #endregion Help Class
    }
}