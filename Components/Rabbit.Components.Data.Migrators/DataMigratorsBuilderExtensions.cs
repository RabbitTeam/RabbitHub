using Autofac;
using Rabbit.Components.Data.Utility.Extensions;
using Rabbit.Kernel;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.Descriptor;
using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;

namespace Rabbit.Components.Data.Migrators
{
    internal sealed class DataMigratorEvents : IShellSettingsManagerEventHandler, IShellDescriptorManagerEventHandler
    {
        #region Field

        public static readonly List<string> NeedMigratorTenants = new List<string>();

        #endregion Field

        #region Implementation of IShellSettingsManagerEventHandler

        /// <summary>
        /// 外壳设置保存成功之后。
        /// </summary>
        /// <param name="settings">外壳设置信息。</param>
        public void Saved(ShellSettings settings)
        {
            if (!DataMigratorsBuilderExtensions.StartingExecute)
                return;

            lock (NeedMigratorTenants)
            {
                if (!NeedMigratorTenants.Contains(settings.Name))
                    NeedMigratorTenants.Add(settings.Name);
            }
        }

        #endregion Implementation of IShellSettingsManagerEventHandler

        #region Implementation of IShellDescriptorManagerEventHandler

        /// <summary>
        /// 当外壳描述符发生变更时执行。
        /// </summary>
        /// <param name="descriptor">新的外壳描述符。</param><param name="tenant">租户名称。</param>
        public void Changed(ShellDescriptor descriptor, string tenant)
        {
            if (!DataMigratorsBuilderExtensions.StartingExecute)
                return;

            lock (NeedMigratorTenants)
            {
                if (!NeedMigratorTenants.Contains(tenant))
                    NeedMigratorTenants.Add(tenant);
            }
        }

        #endregion Implementation of IShellDescriptorManagerEventHandler
    }

    internal class ShellEvents : IShellEvents
    {
        #region Field

        private readonly Lazy<IDataMigratorService> _dataMigratorService;
        private readonly Lazy<ShellSettings> _shellSettings;
        private static readonly object SyncLock = new object();

        #endregion Field

        #region Constructor

        public ShellEvents(Lazy<IDataMigratorService> dataMigratorService, Lazy<ShellSettings> shellSettings)
        {
            _dataMigratorService = dataMigratorService;
            _shellSettings = shellSettings;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Implementation of IShellEvents

        /// <summary>
        /// 激活外壳完成后执行。
        /// </summary>
        public void Activated()
        {
            if (!DataMigratorsBuilderExtensions.StartingExecute)
                return;

            lock (DataMigratorEvents.NeedMigratorTenants)
            {
                if (DataMigratorEvents.NeedMigratorTenants.Contains(_shellSettings.Value.Name))
                {
                    _dataMigratorService.Value.MigrateUp();
                    DataMigratorEvents.NeedMigratorTenants.Remove(_shellSettings.Value.Name);
                }
            }

            lock (SyncLock)
            {
                try
                {
                    var prefix = _shellSettings.Value.GetDataTablePrefix();
                    VersionTable.SetTableName(string.IsNullOrEmpty(prefix) ? "VersionInfo" : $"{prefix}_VersionInfo");
                    _dataMigratorService.Value.MigrateUp();
                }
                catch (Exception exception)
                {
                    Logger.Error("对租户 {0} 进行迁移时发生了一个或多个错误，错误信息：{1}", _shellSettings.Value.Name, exception.Message);
                }
            }
        }

        /// <summary>
        /// 终止外壳前候执行。
        /// </summary>
        public void Terminating()
        {
        }

        #endregion Implementation of IShellEvents
    }

    /// <summary>
    /// 数据建设者扩展方法。
    /// </summary>
    public static class DataMigratorsBuilderExtensions
    {
        internal static bool StartingExecute;

        /// <summary>
        /// 启用数据迁移。
        /// </summary>
        /// <param name="dataBuilder">数据建设者。</param>
        /// <param name="startingExecute">启动时执行。</param>
        public static void EnableDataMigrators(this BuilderExtensions.IDataBuilder dataBuilder, bool startingExecute = true)
        {
            dataBuilder.KernelBuilder
                .RegisterExtension(typeof(DataMigratorsBuilderExtensions).Assembly)
                .OnStarting(builder => builder.RegisterType<DataMigratorEvents>()
                    .As<IShellSettingsManagerEventHandler>()
                    .As<IShellDescriptorManagerEventHandler>()
                    .SingleInstance());

            StartingExecute = startingExecute;
        }
    }
}