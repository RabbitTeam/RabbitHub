using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Extensions.Loaders;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Extensions.Impl
{
    internal sealed class DefaultExtensionMonitoringCoordinator : IExtensionMonitoringCoordinator
    {
        #region Field

        private readonly IVirtualPathMonitor _virtualPathMonitor;
        private readonly IAsyncTokenProvider _asyncTokenProvider;
        private readonly IExtensionManager _extensionManager;
        private readonly IEnumerable<IExtensionLoader> _loaders;

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        public bool Disabled { get; set; }

        #endregion Property

        #region Constructor

        public DefaultExtensionMonitoringCoordinator(IVirtualPathMonitor virtualPathMonitor, IAsyncTokenProvider asyncTokenProvider, IExtensionManager extensionManager, IEnumerable<IExtensionLoader> loaders)
        {
            _virtualPathMonitor = virtualPathMonitor;
            _asyncTokenProvider = asyncTokenProvider;
            _extensionManager = extensionManager;
            _loaders = loaders;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IExtensionMonitoringCoordinator

        /// <summary>
        /// 监控扩展。
        /// </summary>
        /// <param name="monitor">监控动作。</param>
        public void MonitorExtensions(Action<IVolatileToken> monitor)
        {
            if (Disabled)
                return;

            monitor(_asyncTokenProvider.GetToken(MonitorExtensionsWork));
        }

        #endregion Implementation of IExtensionMonitoringCoordinator

        #region Private Method

        private void MonitorExtensionsWork(Action<IVolatileToken> monitor)
        {
            Logger.Information("开始监控扩展文件...");
            //TODO:这边应该由扩展文件夹来提供需要监控的路径而不是固定的模块和主题
            //监控任何在 模块/主题 中的 添加/删除 动作。
            Logger.Debug("监控虚拟路径 \"{0}\"", "~/Modules");
            monitor(_virtualPathMonitor.WhenPathChanges("~/Modules"));
            Logger.Debug("监控虚拟路径 \"{0}\"", "~/Themes");
            monitor(_virtualPathMonitor.WhenPathChanges("~/Themes"));
            Logger.Debug("监控虚拟路径 \"{0}\"", "~/Templates");
            monitor(_virtualPathMonitor.WhenPathChanges("~/Templates"));

            //使用装载机来监控额外的变化。
            var extensions = _extensionManager.AvailableExtensions().ToList();
            foreach (var extension in extensions)
            {
                foreach (var loader in _loaders)
                {
                    loader.Monitor(extension, monitor);
                }
            }
            Logger.Information("完成扩展文件的监控...");
        }

        #endregion Private Method
    }
}