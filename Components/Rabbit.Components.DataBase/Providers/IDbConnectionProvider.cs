using System.Collections.Generic;
using System.Data;

namespace Rabbit.Components.DataBase.Providers
{
    /// <summary>
    /// 一个抽象的数据连接提供程序。
    /// </summary>
    public interface IDbConnectionProvider
    {
        /// <summary>
        /// 数据连接提供程序名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 创建一个数据连接。
        /// </summary>
        /// <param name="parameters">参数。</param>
        /// <returns>数据连接。</returns>
        IDbConnection CreateDbConnection(IDictionary<string, object> parameters);
    }
}