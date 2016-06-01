using MySql.Data.MySqlClient;
using Rabbit.Components.Data.Providers;
using System.Data.Common;

namespace Rabbit.Components.Data.MySql
{
    /// <summary>
    /// MySql数据服务提供程序。
    /// </summary>
    public class MySqlDataServicesProvider : IDataServicesProvider
    {
        #region Implementation of IDataServicesProvider

        /// <summary>创建一个数据库连接。</summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>数据库连接。</returns>
        public DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>提供程序名称。</summary>
        public string ProviderName => "MySql";

        /// <summary>提供程序不变的名称。</summary>
        public string ProviderInvariantName => "MySql.Data.MySqlClient";

        #endregion Implementation of IDataServicesProvider
    }
}