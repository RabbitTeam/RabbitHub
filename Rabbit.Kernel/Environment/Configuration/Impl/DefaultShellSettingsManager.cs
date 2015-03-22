using Rabbit.Kernel.Caching;
using Rabbit.Kernel.FileSystems.AppData;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rabbit.Kernel.Environment.Configuration.Impl
{
    internal sealed class DefaultShellSettingsManager : IShellSettingsManager
    {
        #region Field

        private const string SettingsDirectoryName = "Tenants";
        private const string SettingsFileName = "Settings.txt";
        private readonly IAppDataFolder _appDataFolder;
        private readonly Lazy<IEnumerable<IShellSettingsManagerEventHandler>> _events;
        private readonly ICacheManager _cacheManager;

        private readonly SimpleToken _simpleToken = new SimpleToken();

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Constructor

        public DefaultShellSettingsManager(IAppDataFolder appDataFolder, Lazy<IEnumerable<IShellSettingsManagerEventHandler>> events, ICacheManager cacheManager)
        {
            _appDataFolder = appDataFolder;
            _events = events;
            _cacheManager = cacheManager;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IShellSettingsManager

        /// <summary>
        /// 加载所有外壳设置。
        /// </summary>
        /// <returns>外壳设置集合。</returns>
        IEnumerable<ShellSettings> IShellSettingsManager.LoadSettings()
        {
            return _cacheManager.Get("ShellSettings", ctx =>
            {
                ctx.Monitor(_appDataFolder.WhenPathChanges("~/Tenants"));
                ctx.Monitor(_simpleToken);

                Logger.Debug("读取外壳设置信息...");

                var settingsList = LoadSettingsInternal().ToArray();

                Logger.Debug(() =>
                {
                    var tenantNamesQuery = settingsList.Select(i => i.Name).ToArray();
                    return string.Format("返回了 {0} 个外壳设置信息，来自租户：{1}。", tenantNamesQuery.Length,
                        string.Join(", ", tenantNamesQuery));
                });

                return settingsList;
            });
        }

        /// <summary>
        /// 保存一个外壳设置。
        /// </summary>
        /// <param name="settings">外壳设置信息。</param>
        void IShellSettingsManager.SaveSettings(ShellSettings settings)
        {
            settings.NotNull("settings");

            if (string.IsNullOrWhiteSpace(settings.Name))
                throw new ArgumentException("外壳设置信息中的 Name 属性不能为空。", "settings");

            Logger.Debug("保存租户 '{0}' 的外壳设置信息...", settings.Name);
            var filePath = Path.Combine(SettingsDirectoryName, settings.Name, SettingsFileName);

            Task.Factory.StartNew(
                () => _appDataFolder.CreateFile(filePath, ShellSettingsSerializer.ComposeSettings(settings)));

            Logger.Debug("外壳设置信息保存成功，标记租户 '{0}' 需要重新启动。", settings.Name);
            //使缓存失效。
            _simpleToken.IsCurrent = false;
            if (_events == null)
                return;
            foreach (var @event in _events.Value)
                @event.Saved(settings);
        }

        /// <summary>
        /// 删除一个外壳设置。
        /// </summary>
        /// <param name="name">外壳名称。</param>
        void IShellSettingsManager.DeleteSettings(string name)
        {
            name = name.NotEmptyOrWhiteSpace("name");

            Logger.Debug("删除租户 '{0}' 的外壳设置信息...", name);
            var path = Path.Combine(SettingsDirectoryName, name);
            if (_appDataFolder.DirectoryExists(path))
            {
                Task.Factory.StartNew(() => _appDataFolder.DeleteDirectory(path));
                Logger.Debug("成功删除租户，标记租户 '{0}' 需要释放。", name);
                //使缓存失效。
                _simpleToken.IsCurrent = false;
                if (_events == null)
                    return;
                var settings = new ShellSettings { Name = name, State = TenantState.Disabled };
                foreach (var @event in _events.Value)
                    @event.Saved(settings);
            }
            else
            {
                Logger.Debug("不存在名称为 '{0}' 的租户。", name);
            }
        }

        #endregion Implementation of IShellSettingsManager

        #region Private Method

        private IEnumerable<ShellSettings> LoadSettingsInternal()
        {
            var filePaths = _appDataFolder
                .ListDirectories(SettingsDirectoryName)
                .SelectMany(path => _appDataFolder.ListFiles(path))
                .Where(path => string.Equals(Path.GetFileName(path), SettingsFileName, StringComparison.OrdinalIgnoreCase));

            return filePaths.Select(filePath => ShellSettingsSerializer.ParseSettings(_appDataFolder.ReadFile(filePath)));
        }

        #endregion Private Method

        #region Help Class

        private sealed class SimpleToken : IVolatileToken
        {
            #region Implementation of IVolatileToken

            /// <summary>
            /// 标识缓存是否有效，true为有效，false为失效。
            /// </summary>
            public bool IsCurrent { get; set; }

            #endregion Implementation of IVolatileToken
        }

        #endregion Help Class
    }
}