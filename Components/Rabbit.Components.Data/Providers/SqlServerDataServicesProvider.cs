using System.Data.Common;
using System.Data.SqlClient;

namespace Rabbit.Components.Data.Providers
{
    /// <summary>
    /// SQLServer数据服务提供程序。
    /// </summary>
    public class SqlServerDataServicesProvider : IDataServicesProvider
    {
        #region Implementation of IDataServicesProvider

        /// <summary>
        /// 提供程序名称。
        /// </summary>
        public string ProviderName { get { return "SqlServer"; } }

        /// <summary>
        /// 提供程序不变的名称。
        /// </summary>
        public string ProviderInvariantName { get { return "System.Data.SqlClient"; } }

        /// <summary>
        /// 创建一个数据库连接。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>数据库连接。</returns>
        public DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        #endregion Implementation of IDataServicesProvider
    }
}