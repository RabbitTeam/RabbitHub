using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.FileSystems.Application;
using Rabbit.Kernel.Logging;
using System;
using System.Linq;

namespace Rabbit.Kernel.Tests.Logging
{
    [TestClass]
    public sealed class LoggingTests : TestBase, IDisposable
    {
        #region Property

        public LoggingProxy Proxy { get; set; }

        public IHost Host { get; set; }

        public IApplicationFolder ApplicationFolder { get; set; }

        #endregion Property

        #region Test Method

        [TestMethod]
        public void GlobalLogging()
        {
            Assert.IsNotNull(Proxy.Logger);
        }

        [TestMethod]
        public void ShellLogging()
        {
            try
            {
                Host.Initialize();
            }
            catch (NotSupportedException)
            {
                throw new Exception("已经运行主机的 Initialize 方法后这边是需要重新启动 AppDomain 的所以这边会抛出异常，只运行当前测试是不会出现问题的。");
            }
            var context = Host.GetShellContext(new ShellSettings { Name = "Tenant1" });
            var type = context.Blueprint.Dependencies.First(i => i.Type.Name == "ITest").Type;
            var container = context.Container;
            var service = container.Resolve(type);
            type = service.GetType();
            var value = type.GetProperty("Logger").GetValue(service, null);
            Assert.IsNotNull(value);
        }

        #endregion Test Method

        #region Help Class

        public sealed class LoggingProxy
        {
            public ILogger Logger { get; set; }
        }

        #endregion Help Class

        #region Overrides of TestBase

        protected override void Register(ContainerBuilder builder)
        {
            builder.RegisterType<LoggingProxy>();
        }

        #endregion Overrides of TestBase

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