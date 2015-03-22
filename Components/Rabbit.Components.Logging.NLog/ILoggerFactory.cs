using Rabbit.Kernel.Logging;
using System;

namespace Rabbit.Components.Logging.NLog
{
    /// <summary>
    /// 一个抽象的日志记录器工厂。
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// 创建日志记录器。
        /// </summary>
        /// <param name="type">类型。</param>
        /// <returns>日志记录器实例。</returns>
        ILogger CreateLogger(Type type);
    }
}