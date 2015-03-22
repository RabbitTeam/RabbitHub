using System;

namespace Rabbit.Kernel.Caching
{
    /// <summary>
    /// 一个抽象的缓存管理者。
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// 获取一个缓存实例。
        /// </summary>
        /// <typeparam name="TKey">键类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <returns>缓存实例。</returns>
        ICache<TKey, TResult> GetCache<TKey, TResult>();
    }

    /// <summary>
    /// 缓存管理者扩展方法。
    /// </summary>
    public static class CacheManagerExtensions
    {
        /// <summary>
        /// 根据 <paramref name="key"/> 获取一个缓存值。
        /// </summary>
        /// <typeparam name="TKey">键类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="cacheManager">缓存管理者。</param>
        /// <param name="key">缓存键。</param>
        /// <param name="acquire">获取缓存的动作。</param>
        /// <returns>缓存结果。</returns>
        public static TResult Get<TKey, TResult>(this ICacheManager cacheManager, TKey key,
            Func<AcquireContext<TKey>, TResult> acquire)
        {
            return cacheManager.GetCache<TKey, TResult>().Get(key, acquire);
        }
    }
}