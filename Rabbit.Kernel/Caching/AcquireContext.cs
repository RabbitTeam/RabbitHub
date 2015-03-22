using System;

namespace Rabbit.Kernel.Caching
{
    /// <summary>
    /// 一个抽象的获取上下文。
    /// </summary>
    public interface IAcquireContext
    {
        /// <summary>
        /// 监控动作。
        /// </summary>
        Action<IVolatileToken> Monitor { get; }
    }

    /// <summary>
    /// 默认的获取上下文。
    /// </summary>
    /// <typeparam name="TKey">键类型。</typeparam>
    public sealed class AcquireContext<TKey> : IAcquireContext
    {
        /// <summary>
        /// 初始化一个默认的获取上下文。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="monitor">监控动作。</param>
        public AcquireContext(TKey key, Action<IVolatileToken> monitor)
        {
            Key = key;
            Monitor = monitor;
        }

        /// <summary>
        /// 键。
        /// </summary>
        public TKey Key { get; private set; }

        /// <summary>
        /// 监控动作。
        /// </summary>
        public Action<IVolatileToken> Monitor { get; private set; }
    }

    /// <summary>
    /// 一个简单的获取上下文。
    /// </summary>
    public sealed class SimpleAcquireContext : IAcquireContext
    {
        private readonly Action<IVolatileToken> _monitor;

        /// <summary>
        /// 初始化一个简单的获取上下文。
        /// </summary>
        /// <param name="monitor">监控动作。</param>
        public SimpleAcquireContext(Action<IVolatileToken> monitor)
        {
            _monitor = monitor;
        }

        /// <summary>
        /// 监控动作。
        /// </summary>
        public Action<IVolatileToken> Monitor
        {
            get { return _monitor; }
        }
    }
}