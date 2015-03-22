using System;

namespace Rabbit.Kernel.Caching.Impl
{
    internal sealed class DefaultCacheManager : ICacheManager
    {
        #region Field

        private readonly Type _component;
        private readonly ICacheHolder _cacheHolder;

        #endregion Field

        #region Constructor

        public DefaultCacheManager(Type component, ICacheHolder cacheHolder)
        {
            _component = component;
            _cacheHolder = cacheHolder;
        }

        #endregion Constructor

        #region Implementation of ICacheManager

        /// <summary>
        /// 获取一个缓存实例。
        /// </summary>
        /// <typeparam name="TKey">键类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <returns>缓存实例。</returns>
        public ICache<TKey, TResult> GetCache<TKey, TResult>()
        {
            return _cacheHolder.GetCache<TKey, TResult>(_component);
        }

        #endregion Implementation of ICacheManager
    }
}