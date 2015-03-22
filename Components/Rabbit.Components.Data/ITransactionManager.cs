using Rabbit.Kernel;
using System.Data;

namespace Rabbit.Components.Data
{
    /// <summary>
    /// 一个抽象的事务管理者。
    /// </summary>
    public interface ITransactionManager : IDependency
    {
        /// <summary>
        /// 索取一个事务。
        /// </summary>
        void Demand();

        /// <summary>
        /// 索取一个锁行为为 <see cref="IsolationLevel.ReadCommitted"/> 的新事务。
        /// </summary>
        void RequireNew();

        /// <summary>
        /// 索取一个锁行为为 <paramref name="level"/> 的新事务。
        /// </summary>
        /// <param name="level">事务锁定行为。</param>
        void RequireNew(IsolationLevel level);

        /// <summary>
        /// 取消事务。
        /// </summary>
        void Cancel();
    }
}