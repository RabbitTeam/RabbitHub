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
using Rabbit.Kernel;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit.Components.Data.Migrators
{
    /// <summary>
    /// 一个抽象的数据迁移服务。
    /// </summary>
    public interface IDataMigratorService : IDependency
    {
        /// <summary>
        /// 对所有租户执行数据迁移。
        /// </summary>
        /// <param name="useAutomaticTransactionManagement">使用自动事物管理。</param>
        void MigrateUp(bool useAutomaticTransactionManagement = true);

        /// <summary>
        /// 对所有租户执行指定版本的数据迁移。
        /// </summary>
        /// <param name="targetVersion">目标版本。</param>
        /// <param name="useAutomaticTransactionManagement">使用自动事物管理。</param>
        void MigrateUp(long targetVersion, bool useAutomaticTransactionManagement = true);

        /// <summary>
        /// 对指定租户执行回滚。
        /// </summary>
        /// <param name="version">版本。</param>
        /// <param name="useAutomaticTransactionManagement">使用自动事物管理。</param>
        void RollbackToVersion(long version, bool useAutomaticTransactionManagement);
    }

    internal sealed class DataMigratorService : IDataMigratorService
    {
        #region Field

        private readonly IEnumerable<IMigratorDataServicesProvider> _migratorDataServicesProviders;
        private readonly IEnumerable<IMigration> _migrations;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ShellSettings _shellSettings;

        #endregion Field

        #region Constructor

        public DataMigratorService(IEnumerable<IMigratorDataServicesProvider> migratorDataServicesProviders, IEnumerable<IMigration> migrations, ILifetimeScope lifetimeScope, ShellSettings shellSettings)
        {
            _migratorDataServicesProviders = migratorDataServicesProviders;
            _migrations = migrations;
            _lifetimeScope = lifetimeScope;
            _shellSettings = shellSettings;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Implementation of IDataMigratorService

        /// <summary>
        /// 对所有租户执行数据迁移。
        /// </summary>
        /// <param name="useAutomaticTransactionManagement">使用自动事物管理。</param>
        public void MigrateUp(bool useAutomaticTransactionManagement = true)
        {
            Execute(runner => runner.MigrateUp(useAutomaticTransactionManagement));
        }

        /// <summary>
        /// 对所有租户执行指定版本的数据迁移。
        /// </summary>
        /// <param name="targetVersion">目标版本。</param>
        /// <param name="useAutomaticTransactionManagement">使用自动事物管理。</param>
        public void MigrateUp(long targetVersion, bool useAutomaticTransactionManagement = true)
        {
            Execute(runner => runner.MigrateUp(targetVersion, useAutomaticTransactionManagement));
        }

        /// <summary>
        /// 对指定租户执行回滚。
        /// </summary>
        /// <param name="version">版本。</param>
        /// <param name="useAutomaticTransactionManagement">使用自动事物管理。</param>
        public void RollbackToVersion(long version, bool useAutomaticTransactionManagement)
        {
            Execute(runner => runner.RollbackToVersion(version, useAutomaticTransactionManagement));
        }

        private void Execute(Action<MigrationRunner> action)
        {
            //数据服务提供程序。
            var dataServicesProviders = _migratorDataServicesProviders;
            var tenant = _shellSettings;
            //转换类型。
            var migrations = _migrations.Select(i =>
            {
                var item = i as MigrationBase;
                if (item == null)
                    return null;
                //注入容器。
                item.LifetimeScope = _lifetimeScope;
                return item;
            }).Where(i => i != null).ToArray();

            Logger.Information("准备对租户 {0} 进行数据迁移...", tenant.Name);

            var dataProviderName = tenant.GetDataProvider();
            var connectionString = tenant.GetDataConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Logger.Warning(string.Format("未对租户 {0} 进行数据迁移，因为没有配置数据服务提供程序。", tenant.Name));
                return;
            }
            if (string.IsNullOrWhiteSpace(dataProviderName))
            {
                Logger.Warning(string.Format("未对租户 {0} 进行数据迁移，因为没有配置数据连接字符串。", tenant.Name));
                return;
            }
            var provider = dataServicesProviders.SingleOrDefault(i => i.ProviderName.Equals(dataProviderName));

            if (provider == null)
            {
                Logger.Debug(string.Format("未对租户 {0} 进行数据迁移，因为根据数据服务提供程序名称 {1}，找不到对应的数据服务提供程序。", tenant.Name, dataProviderName));
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
                        MigrationLoader = new ChunSunMigrationInformationLoader(migrations, Logger)
                    };
                    action(migrationRunner);
                }
            }

            Logger.Information("完成对租户 {0} 的数据迁移", tenant.Name);
        }

        #endregion Implementation of IDataMigratorService

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