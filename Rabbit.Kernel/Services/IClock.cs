using Rabbit.Kernel.Caching;
using System;

namespace Rabbit.Kernel.Services
{
    /// <summary>
    /// 提供UTC时间支持。
    /// </summary>
    public interface IClock : IVolatileProvider
    {
        /// <summary>
        /// 从 <see cref="DateTime"/> 获取系统当前的Utc时间。
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// 根据绝对Utc时间隔获取一个挥发令牌。
        /// </summary>
        /// <param name="absoluteUtc">绝对的Utc时间。</param>
        /// <returns>挥发令牌实例。</returns>
        IVolatileToken WhenUtc(DateTime absoluteUtc);
    }

    /// <summary>
    /// 时钟扩展方法。
    /// </summary>
    public static class ClockExtensions
    {
        /// <summary>
        /// 根据时间间隔获取一个挥发令牌。
        /// </summary>
        /// <param name="clock">时钟服务。</param>
        /// <param name="duration">时间间隔。</param>
        /// <returns>挥发令牌实例。</returns>
        public static IVolatileToken When(this IClock clock, TimeSpan duration)
        {
            return clock.WhenUtc(clock.UtcNow.Add(duration));
        }
    }
}