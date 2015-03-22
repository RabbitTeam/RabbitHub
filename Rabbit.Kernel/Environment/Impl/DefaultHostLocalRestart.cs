using Rabbit.Kernel.Caching;
using Rabbit.Kernel.FileSystems.AppData;
using Rabbit.Kernel.Logging;
using System;

namespace Rabbit.Kernel.Environment.Impl
{
    internal sealed class DefaultHostLocalRestart : IHostLocalRestart
    {
        #region Field

        private readonly IAppDataFolder _appDataFolder;
        private const string FileName = "hrestart.txt";

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Constructor

        public DefaultHostLocalRestart(IAppDataFolder appDataFolder)
        {
            _appDataFolder = appDataFolder;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IHostLocalRestart

        /// <summary>
        /// 监控动作。
        /// </summary>
        /// <param name="monitor">监控动作。</param>
        public void Monitor(Action<IVolatileToken> monitor)
        {
            if (!_appDataFolder.FileExists(FileName))
                TouchFile();

            Logger.Debug("监控虚拟路径 \"{0}\"", FileName);
            monitor(_appDataFolder.WhenPathChanges(FileName));
        }

        #endregion Implementation of IHostLocalRestart

        #region Private Method

        private void TouchFile()
        {
            try
            {
                _appDataFolder.CreateFile(FileName, "Host Restart");
            }
            catch (Exception e)
            {
                Logger.Warning(e, "更新文件 '{0}' 时发生了错误。", FileName);
            }
        }

        #endregion Private Method
    }
}