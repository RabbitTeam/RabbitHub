using Rabbit.Kernel.Logging;
using System;

namespace Rabbit.Components.Logging.NLog
{
    internal sealed class LoggerFactory : ILoggerFactory
    {
        #region Field

        private readonly ILoggingConfigurationResolve _loggingConfigurationResolve;

        #endregion Field

        #region Constructor

        public LoggerFactory(ILoggingConfigurationResolve loggingConfigurationResolve)
        {
            _loggingConfigurationResolve = loggingConfigurationResolve;
        }

        #endregion Constructor

        #region Implementation of ILoggerFactory

        /// <summary>
        /// 创建日志记录器。
        /// </summary>
        /// <param name="type">类型。</param>
        /// <returns>日志记录器实例。</returns>
        public ILogger CreateLogger(Type type)
        {
            return new NLogLogger(type, _loggingConfigurationResolve);
        }

        #endregion Implementation of ILoggerFactory
    }
}