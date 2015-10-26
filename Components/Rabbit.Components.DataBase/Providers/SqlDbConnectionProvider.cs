using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Rabbit.Components.DataBase.Providers
{
    internal sealed class SqlDbConnectionProvider : IDbConnectionProvider
    {
        #region Implementation of IDbConnectionProvider

        /// <summary>
        /// 数据连接提供程序名称。
        /// </summary>
        public string Name => "SQLServer";

        /// <summary>
        /// 创建一个数据连接。
        /// </summary>
        /// <param name="parameters">参数。</param>
        /// <returns>数据连接。</returns>
        public IDbConnection CreateDbConnection(IDictionary<string, object> parameters)
        {
            const string connectionStringKey = "DataConnectionString";
            if (!parameters.ContainsKey(connectionStringKey))
            {
                throw new ArgumentException($"找不到必要的参数 \"{connectionStringKey}\"。");
            }
            var connectionString = parameters[connectionStringKey] as string;
            if (connectionString == null)
                throw new ArgumentException($"\"{connectionStringKey}\" 必须是一个字符串。");

            return new SqlConnection(connectionString);
        }

        #endregion Implementation of IDbConnectionProvider
    }
}