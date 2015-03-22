using System;

namespace Rabbit.Kernel.Caching.Impl
{
    internal sealed class DefaultCacheContextAccessor : ICacheContextAccessor
    {
        [ThreadStatic]
        private static IAcquireContext _threadInstance;

        private static IAcquireContext ThreadInstance
        {
            get { return _threadInstance; }
            set { _threadInstance = value; }
        }

        #region Implementation of ICacheContextAccessor

        /// <summary>
        /// 当前获取上下文。
        /// </summary>
        public IAcquireContext Current
        {
            get { return ThreadInstance; }
            set { ThreadInstance = value; }
        }

        #endregion Implementation of ICacheContextAccessor
    }
}