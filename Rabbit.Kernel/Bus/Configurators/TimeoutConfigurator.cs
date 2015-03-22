using Rabbit.Kernel.Bus.Exceptions;
using System;

namespace Rabbit.Kernel.Bus.Configurators
{
    /// <summary>
    /// 超时配置。
    /// </summary>
    public sealed class TimeoutConfigurator
    {
        /// <summary>
        /// 初始化一个新的超时配置。
        /// </summary>
        public TimeoutConfigurator()
        {
            TimeoutTime = new TimeSpan(0, 0, 10);
        }

        /// <summary>
        /// 超时时间（默认为10秒）。
        /// </summary>
        public TimeSpan TimeoutTime { get; private set; }

        /// <summary>
        /// 超时之后的动作，返回true则不抛出异常，否则抛出异常。
        /// </summary>
        internal Func<RequestTimeoutException, bool> TimeoutAction { get; private set; }

        /// <summary>
        /// 设置请求超时的时间。
        /// </summary>
        /// <param name="timeSpan">超时时间。</param>
        /// <returns>请求配置。</returns>
        public TimeoutConfigurator Timeout(TimeSpan timeSpan)
        {
            TimeoutTime = timeSpan;

            return this;
        }

        /// <summary>
        /// 设置超时之后的动作。
        /// </summary>
        /// <param name="timeout">超时之后的动作，返回true则不抛出异常，否则抛出异常。</param>
        /// <returns>请求配置。</returns>
        public TimeoutConfigurator Handle(Func<RequestTimeoutException, bool> timeout)
        {
            TimeoutAction = timeout;
            return this;
        }
    }
}