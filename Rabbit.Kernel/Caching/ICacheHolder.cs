using System;

namespace Rabbit.Kernel.Caching
{
    /// <summary>
    /// 一个抽象的缓存持有者。
    /// </summary>
    public interface ICacheHolder : ISingletonDependency
    {
        /// <summary>
        /// 获取缓存。
        /// </summary>
        /// <typeparam name="TKey">键类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="component">组件类型。</param>
        /// <returns>缓存实例。</returns>
        ICache<TKey, TResult> GetCache<TKey, TResult>(Type component);
    }
}