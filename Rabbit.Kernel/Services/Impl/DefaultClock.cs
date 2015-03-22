using Rabbit.Kernel.Caching;
using System;

namespace Rabbit.Kernel.Services.Impl
{
    internal sealed class DefaultClock : IClock
    {
        #region Implementation of IClock

        /// <summary>
        /// 从 <see cref="DateTime"/> 获取系统当前的Utc时间。
        /// </summary>
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        /// <summary>
        /// 根据绝对Utc时间隔获取一个挥发令牌。
        /// </summary>
        /// <param name="absoluteUtc">绝对的Utc时间。</param>
        /// <returns>挥发令牌实例。</returns>
        public IVolatileToken WhenUtc(DateTime absoluteUtc)
        {
            return new AbsoluteExpirationToken(this, absoluteUtc);
        }

        #endregion Implementation of IClock

        #region Help Class

        private sealed class AbsoluteExpirationToken : IVolatileToken
        {
            #region Field

            private readonly IClock _clock;
            private readonly DateTime _invalidateUtc;

            #endregion Field

            #region Constructor

            public AbsoluteExpirationToken(IClock clock, DateTime invalidateUtc)
            {
                _clock = clock;
                _invalidateUtc = invalidateUtc;
            }

            #endregion Constructor

            #region Implementation of IVolatileToken

            /// <summary>
            /// 标识缓存是否有效，true为有效，false为失效。
            /// </summary>
            public bool IsCurrent
            {
                get { return _clock.UtcNow < _invalidateUtc; }
            }

            #endregion Implementation of IVolatileToken
        }

        #endregion Help Class
    }
}