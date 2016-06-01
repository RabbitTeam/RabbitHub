using FluentMigrator;
using FluentMigrator.Runner.Generators.MySql;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.MySql;
using Rabbit.Components.Data.Migrators.Providers;
using Rabbit.Components.Data.MySql;
using System.Data;

namespace Rabbit.Components.Data.Migrators.MySql
{
    /// <summary>
    /// MySql数据迁移服务提供程序。
    /// </summary>
    public class MySqlMigratorDataServicesProvider : MySqlDataServicesProvider, IMigratorDataServicesProvider
    {
        #region Implementation of IMigratorDataServicesProvider

        /// <summary>
        /// 创建迁移处理器。
        /// </summary>
        /// <param name="context">运行上下文。</param>
        /// <param name="connection">数据库连接。</param>
        /// <returns>迁移处理器。</returns>
        public IMigrationProcessor CreateMigrationProcessor(RunnerContext context, IDbConnection connection)
        {
            return new MySqlProcessor(connection, new MySqlGenerator(), context.Announcer, new ProcessorOptions
            {
                PreviewOnly = context.PreviewOnly,
                ProviderSwitches = context.ProviderSwitches,
                Timeout = context.Timeout
            }, new MySqlDbFactory());
        }

        #endregion Implementation of IMigratorDataServicesProvider
    }
}