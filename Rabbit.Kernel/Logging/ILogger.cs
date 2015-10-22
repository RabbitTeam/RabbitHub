using System;

namespace Rabbit.Kernel.Logging
{
    /// <summary>
    /// 日志等级。
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 追踪。
        /// </summary>
        Trace,

        /// <summary>
        /// 调试。
        /// </summary>
        Debug,

        /// <summary>
        /// 信息。
        /// </summary>
        Information,

        /// <summary>
        /// 警告。
        /// </summary>
        Warning,

        /// <summary>
        /// 错误。
        /// </summary>
        Error,

        /// <summary>
        /// 致命错误。
        /// </summary>
        Fatal
    }

    /// <summary>
    /// 一个抽象的日志记录器接口。
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 判断日志记录器是否开启。
        /// </summary>
        /// <param name="level">日志等级。</param>
        /// <returns>如果开启返回true，否则返回false。</returns>
        bool IsEnabled(LogLevel level);

        /// <summary>
        /// 记录日志。
        /// </summary>
        /// <param name="level">日志等级。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">格式。</param>
        /// <param name="args">参数。</param>
        void Log(LogLevel level, Exception exception, string format, params object[] args);
    }
}