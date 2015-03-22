using FluentMigrator;
using FluentMigrator.Runner.Initialization;
using Rabbit.Components.Data.Providers;
using System.Data;

namespace Rabbit.Components.Data.Migrators.Providers
{
    internal interface IMigratorDataServicesProvider : IDataServicesProvider
    {
        IMigrationProcessor CreateMigrationProcessor(RunnerContext context, IDbConnection connection);
    }
}