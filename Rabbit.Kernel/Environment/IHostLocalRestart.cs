using Rabbit.Kernel.Caching;
using System;

namespace Rabbit.Kernel.Environment
{
    /// <summary>
    /// 一个抽象的本地主机重启器。
    /// </summary>
    public interface IHostLocalRestart
    {
        /// <summary>
        /// 监控动作。
        /// </summary>
        /// <param name="monitor">监控动作。</param>
        void Monitor(Action<IVolatileToken> monitor);
    }
}