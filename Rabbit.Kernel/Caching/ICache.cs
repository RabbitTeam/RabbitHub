using System;

namespace Rabbit.Kernel.Caching
{
    /// <summary>
    /// 一个抽象的缓存。
    /// </summary>
    /// <typeparam name="TKey">键类型。</typeparam>
    /// <typeparam name="TResult">值类型。</typeparam>
    public interface ICache<TKey, TResult>
    {
        /// <summary>
        /// 根据键获取一个缓存结果。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="acquire">获取上下文。</param>
        /// <returns>缓存结果。</returns>
        TResult Get(TKey key, Func<AcquireContext<TKey>, TResult> acquire);
    }
}