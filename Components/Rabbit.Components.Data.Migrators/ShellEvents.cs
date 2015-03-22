using Autofac;
using FluentMigrator;
using FluentMigrator.Exceptions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using Rabbit.Components.Data.Migrators.Providers;
using Rabbit.Components.Data.Utility.Extensions;
using Rabbit.Kernel.Environment;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit.Components.Data.Migrators
{
    internal sealed class ShellEvents : IShellEvents
    {
        #region Field

        private readonly IEnumerable<MigrationBase> _migrations;
        private readonly ShellSettings _shellSettings;
        private readonly IEnumerable<IMigratorDataServicesProvider> _dataServicesProviders;

        #endregion Field

        #region Constructor

        public ShellEvents(ILifetimeScope lifetimeScope)
        {
            //数据服务提供程序。
            _dataServicesProviders = lifetimeScope.Resolve<IEnumerable<IMigratorDataServicesProvider>>();

            //转换类型。
            _migrations = lifetimeScope.Resolve<IEnumerable<IMigration>>().Select(i =>
            {
                var item = i as MigrationBase;
                if (item == null)
                    return null;
                //注入容器。
                item.LifetimeScope = lifetimeScope;
                return item;
            }).Where(i => i != null).ToArray();

            _shellSettings = lifetimeScope.Resolve<ShellSettings>();

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
            Logger.Information("准备对租户 {0} 进行数据迁移...", _shellSettings.Name);

            var dataProviderName = _shellSettings.GetDataProvider();
            var connectionString = _shellSettings.GetDataConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Logger.Warning(string.Format("未对租户 {0} 进行数据迁移，因为没有配置数据服务提供程序。", _shellSettings.Name));
                return;
            }
            if (string.IsNullOrWhiteSpace(dataProviderName))
            {
                Logger.Warning(string.Format("未对租户 {0} 进行数据迁移，因为没有配置数据连接字符串。", _shellSettings.Name));
                return;
            }
            var provider = _dataServicesProviders.SingleOrDefault(i => i.ProviderName.Equals(dataProviderName));

            if (provider == null)
            {
                Logger.Debug(string.Format("未对租户 {0} 进行数据迁移，因为根据数据服务提供程序名称 {1}，找不到对应的数据服务提供程序。", _shellSettings.Name, dataProviderName));
                return;
            }

            using (var connection = provider.CreateConnection(connectionString))
            {
                var context = new RunnerContext(new NullAnnouncer());
                var migrationProcessor = provider.CreateMigrationProcessor(context, connection);

                using (migrationProcessor)
                {
                    var migrationRunner = new MigrationRunner(GetType().Assembly, context, migrationProcessor)
                    {
                        MigrationLoader = new ChunSunMigrationInformationLoader(_migrations, Logger)
                    };
                    //迁移。
                    migrationRunner.MigrateUp(true);
                }
            }

            Logger.Information("完成对租户 {0} 的数据迁移", _shellSettings.Name);
        }

        /// <summary>
        /// 终止外壳前候执行。
        /// </summary>
        public void Terminating()
        {
        }

        #endregion Implementation of IShellEvents

        #region Help Class

        internal sealed class ChunSunMigrationInformationLoader : IMigrationInformationLoader
        {
            #region Field

            private readonly IEnumerable<FluentMigrator.IMigration> _migrations;
            private readonly ILogger _logger;

            #endregion Field

            #region Constructor

            public ChunSunMigrationInformationLoader(IEnumerable<IMigration> migrations, ILogger logger)
            {
                _logger = logger;
                _migrations = migrations.Cast<FluentMigrator.IMigration>();
            }

            #endregion Constructor

            #region Implementation of IMigrationInformationLoader

            public SortedList<long, IMigrationInfo> LoadMigrations()
            {
                var migrationInfos = new SortedList<long, IMigrationInfo>();
                var builder = new StringBuilder("加载了以下迁移程序：");
                builder.AppendLine(string.Empty);
                foreach (var migrationInfo in _migrations.Select(GetMigrationInfoFor))
                {
                    if (migrationInfos.ContainsKey(migrationInfo.Version))
                        throw new DuplicateMigrationException(string.Format("发现重复的迁移版本 {0}.", migrationInfo.Version));
                    migrationInfos.Add(migrationInfo.Version, migrationInfo);
                    builder.AppendFormat("版本：{0}，说明：{1}", migrationInfo.Version, migrationInfo.Description);
                }
                _logger.Information(builder.ToString());
                return migrationInfos;
            }

            #endregion Implementation of IMigrationInformationLoader

            private static IMigrationInfo GetMigrationInfoFor(FluentMigrator.IMigration migration)
            {
                var migrationType = migration.GetType();
                var migrationAttribute = migrationType.GetOneAttribute<MigrationAttribute>();
                var chunSunMigration = migration as MigrationBase;
                if (chunSunMigration == null)
                    throw new Exception("迁移提供程序必须继承自 \"" + typeof(MigrationBase).FullName + "\" 类型。");
                var id = chunSunMigration.Feature.Descriptor.Id;

                var version = migrationAttribute.Version;
                var description = id + "_" + migrationAttribute.Description + "_" + migrationAttribute.Version;

                var migrationInfo = new MigrationInfo(version, description, migrationAttribute.TransactionBehavior, () => migration);
                var allAttributes = migrationType.GetAllAttributes<MigrationTraitAttribute>();
                foreach (var traitAttribute in allAttributes)
                    migrationInfo.AddTrait(traitAttribute.Name, traitAttribute.Value);
                return migrationInfo;
            }
        }

        #endregion Help Class
    }
}