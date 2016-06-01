using FluentMigrator;
using FluentMigrator.Runner.Initialization;
using Rabbit.Components.Data.Providers;
using System.Data;

namespace Rabbit.Components.Data.Migrators.Providers
{
    /// <summary>
    /// 一个抽象的数据迁移服务提供程序。
    /// </summary>
    public interface IMigratorDataServicesProvider : IDataServicesProvider
    {
        /// <summary>
        /// 创建迁移处理器。
        /// </summary>
        /// <param name="context">运行上下文。</param>
        /// <param name="connection">数据库连接。</param>
        /// <returns>迁移处理器。</returns>
        IMigrationProcessor CreateMigrationProcessor(RunnerContext context, IDbConnection connection);
    }
}