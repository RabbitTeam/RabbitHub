using MySql.Data.MySqlClient;
using Rabbit.Components.Data.EntityFramework.Providers;
using Rabbit.Components.Data.MySql;
using System.Data.Entity.Core.Common;

namespace Rabbit.Components.Data.EntityFramework.MySql
{
    /// <summary>
    /// MySql EntityFramework 数据服务提供程序。
    /// </summary>
    public class MySqlEntityFrameworkDataServicesProvider : MySqlDataServicesProvider, IEntityFrameworkDataServicesProvider
    {
        private static readonly MySqlProviderServices ProviderInstance = new MySqlProviderServices();

        #region Implementation of IEntityFrameworkDataServicesProvider

        /// <summary>
        /// 数据库提供服务。
        /// </summary>
        public DbProviderServices Instance => ProviderInstance;

        #endregion Implementation of IEntityFrameworkDataServicesProvider
    }
}