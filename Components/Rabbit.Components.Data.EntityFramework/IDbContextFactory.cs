using Rabbit.Kernel;
using System.Data.Entity;

namespace Rabbit.Components.Data.EntityFramework
{
    internal interface IDbContextFactory : IDependency
    {
        DbContext CreateDbContext();
    }
}