using Rabbit.Kernel;
using System.Data.Common;

namespace Rabbit.Components.Data.Providers
{
    /// <summary>
    /// 一个抽象的数据服务提供程序。
    /// </summary>
    public interface IDataServicesProvider : ITransientDependency
    {
        /// <summary>
        /// 提供程序名称。
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// 提供程序不变的名称。
        /// </summary>
        string ProviderInvariantName { get; }

        /// <summary>
        /// 创建一个数据库连接。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>数据库连接。</returns>
        DbConnection CreateConnection(string connectionString);
    }
}