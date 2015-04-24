using Rabbit.Components.Data.EntityFramework.Migrations;
using Rabbit.Web.Works;
using System.Data.Entity;
using System.Web;

namespace Rabbit.Components.Data.EntityFramework.Impl
{
    internal sealed class DefaultDbConfiguration : DbConfiguration
    {
        public DefaultDbConfiguration()
        {
            IDatabaseInitializer<DefaultDbContext> databaseInitializer;

            if (GlobalConfig.AutomaticMigrationsEnabled)
            {
                databaseInitializer = new MigrateDatabaseToLatestVersion<DefaultDbContext, Configuration>();
                SetContextFactory(typeof(DefaultDbContext), () =>
                {
                    var work = new HttpContextWrapper(HttpContext.Current).Request.RequestContext.GetWorkContext();
                    return work.Resolve<EntityFrameworkDbContextFactory>().Create();
                });
            }
            else
                databaseInitializer = new NullDatabaseInitializer<DefaultDbContext>();

            SetDatabaseInitializer(databaseInitializer);
        }
    }
}