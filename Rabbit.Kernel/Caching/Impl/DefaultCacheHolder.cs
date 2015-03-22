using System;
using System.Collections.Concurrent;

namespace Rabbit.Kernel.Caching.Impl
{
    internal sealed class DefaultCacheHolder : ICacheHolder
    {
        #region Field

        private readonly ICacheContextAccessor _cacheContextAccessor;
        private readonly ConcurrentDictionary<CacheKey, object> _caches = new ConcurrentDictionary<CacheKey, object>();

        #endregion Field

        #region Constructor

        public DefaultCacheHolder(ICacheContextAccessor cacheContextAccessor)
        {
            _cacheContextAccessor = cacheContextAccessor;
        }

        #endregion Constructor

        #region Implementation of ICacheHolder

        /// <summary>
        /// 获取缓存。
        /// </summary>
        /// <typeparam name="TKey">键类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="component">组件类型。</param>
        /// <returns>缓存实例。</returns>
        public ICache<TKey, TResult> GetCache<TKey, TResult>(Type component)
        {
            var cacheKey = new CacheKey(component, typeof(TKey), typeof(TResult));
            var result = _caches.GetOrAdd(cacheKey, k => new Cache<TKey, TResult>(_cacheContextAccessor));
            return (Cache<TKey, TResult>)result;
        }

        #endregion Implementation of ICacheHolder

        #region Help Class

        private sealed class CacheKey : Tuple<Type, Type, Type>
        {
            public CacheKey(Type component, Type key, Type result)
                : base(component, key, result)
            {
            }
        }

        #endregion Help Class
    }
}