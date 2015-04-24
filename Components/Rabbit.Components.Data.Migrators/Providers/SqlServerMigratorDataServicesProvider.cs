using FluentMigrator;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;
using Rabbit.Components.Data.Providers;
using System.Data;
using System.Data.SqlClient;

namespace Rabbit.Components.Data.Migrators.Providers
{
    internal sealed class SqlServerMigratorDataServicesProvider : SqlServerDataServicesProvider, IMigratorDataServicesProvider
    {
        #region Implementation of IMigratorDataServicesProvider

        public IMigrationProcessor CreateMigrationProcessor(RunnerContext context, IDbConnection connection)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                IMigrationGenerator migrationGenerator;
                using (var command = new SqlCommand("SELECT CAST(LEFT(CAST(SERVERPROPERTY('productversion') AS varchar(50)),CHARINDEX('.',CAST(SERVERPROPERTY('productversion') AS varchar(50)))-1)AS int)", connection as SqlConnection))
                {
                    var version = (int)command.ExecuteScalar();
                    switch (version)
                    {
                        case 9:
                            migrationGenerator = new SqlServer2005Generator();
                            break;

                        case 10:
                            migrationGenerator = new SqlServer2008Generator();
                            break;

                        default:
                            migrationGenerator = new SqlServer2012Generator();
                            //                            throw new Exception(string.Format("无法识别的数据库版本：{0}。", version));
                            break;
                    }
                }

                return new SqlServerProcessor(
                                        connection
                                        , migrationGenerator
                                        , context.Announcer,
                                        new ProcessorOptions
                                        {
                                            PreviewOnly = context.PreviewOnly,
                                            ProviderSwitches = context.ProviderSwitches,
                                            Timeout = context.Timeout
                                        }
                                        , new SqlServerDbFactory());
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        #endregion Implementation of IMigratorDataServicesProvider
    }
}