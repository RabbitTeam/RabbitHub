using System;

namespace Rabbit.Kernel.Logging
{
    /// <summary>
    /// 一个空的日志记录器。
    /// </summary>
    public class NullLogger : ILogger
    {
        #region Field

        private static readonly ILogger Logger = new NullLogger();

        #endregion Field

        #region Property

        /// <summary>
        /// 记录器实例。
        /// </summary>
        public static ILogger Instance
        {
            get { return Logger; }
        }

        #endregion Property

        #region Implementation of ILogger

        /// <summary>
        /// 判断日志记录器是否开启。
        /// </summary>
        /// <param name="level">日志等级。</param>
        /// <returns>如果开启返回true，否则返回false。</returns>
        public bool IsEnabled(LogLevel level)
        {
            return false;
        }

        /// <summary>
        /// 记录日志。
        /// </summary>
        /// <param name="level">日志等级。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">格式。</param>
        /// <param name="args">参数。</param>
        public void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
        }

        #endregion Implementation of ILogger
    }
}