using Rabbit.Components.Data.Providers;
using System.Data.Entity.Core.Common;
using System.Data.Entity.SqlServer;

namespace Rabbit.Components.Data.EntityFramework.Providers
{
    internal sealed class SqlServerEntityFrameworkDataServicesProvider : SqlServerDataServicesProvider, IEntityFrameworkDataServicesProvider
    {
        #region Implementation of IEntityFrameworkDataServicesProvider

        public DbProviderServices Instance { get { return SqlProviderServices.Instance; } }

        #endregion Implementation of IEntityFrameworkDataServicesProvider
    }
}