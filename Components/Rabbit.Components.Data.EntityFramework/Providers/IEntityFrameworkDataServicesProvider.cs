using Rabbit.Components.Data.Providers;
using System.Data.Entity.Core.Common;

namespace Rabbit.Components.Data.EntityFramework.Providers
{
    /// <summary>
    /// 一个抽象的EntityFramework数据服务提供程序。
    /// </summary>
    public interface IEntityFrameworkDataServicesProvider : IDataServicesProvider
    {
        /// <summary>
        /// 数据库提供服务。
        /// </summary>
        DbProviderServices Instance { get; }
    }
}