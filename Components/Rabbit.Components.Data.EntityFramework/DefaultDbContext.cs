using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Rabbit.Components.Data.EntityFramework
{
    internal sealed class DefaultDbContext : DbContext
    {
        public DefaultDbContext(string nameOrConnectionString, DbCompiledModel dbCompiledModel)
            : base(nameOrConnectionString, dbCompiledModel)
        {
        }
    }
}