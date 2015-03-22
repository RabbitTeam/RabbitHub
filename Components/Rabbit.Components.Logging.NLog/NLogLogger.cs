using NLog;
using NLog.Config;
using Rabbit.Kernel.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using LogLevel = Rabbit.Kernel.Logging.LogLevel;

namespace Rabbit.Components.Logging.NLog
{
    internal class DetailedException : Exception
    {
        public DetailedException(Exception exception)
            : base(GetMessage(exception))
        {
        }

        private static string GetMessage(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var builder = new StringBuilder();

            while (true)
            {
                if (exception == null)
                    break;

                if (exception is ReflectionTypeLoadException)
                {
                    var reflectionTypeLoadException = exception as ReflectionTypeLoadException;

                    builder.Append(string.Join("|", reflectionTypeLoadException.LoaderExceptions.Select(i => i.Message)));
                }
                else
                {
                    builder.Append("|").Append(exception.Message);
                }

                if (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    continue;
                }
                break;
            }

            return builder.ToString();
        }
    }

    internal sealed class NLogLogger : ILogger
    {
        #region Field

        private readonly Logger _logger;

        #endregion Field

        #region Constructor

        public NLogLogger(Type type, ILoggingConfigurationResolve loggingConfigurationResolve)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            var assembly = type.Assembly;

            var loggingConfiguration = loggingConfigurationResolve.GetLoggingConfiguration(assembly);
            using (var logFactory = new LogFactory(loggingConfiguration as LoggingConfiguration))
            {
                _logger = logFactory.GetLogger(type.FullName);
            }
        }

        #endregion Constructor

        #region Implementation of ILogger

        /// <summary>
        /// 当前日志等级是否可以记录。
        /// </summary>
        /// <param name="level">日志等级。</param>
        /// <returns>是否可以记录。</returns>
        public bool IsEnabled(LogLevel level)
        {
            return _logger.IsEnabled(LogUtilities.ConvertLogLevel(level));
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
            var logLevel = LogUtilities.ConvertLogLevel(level);

            var logEventInfo = LogEventInfo.Create(logLevel, _logger.Name, CultureInfo.CurrentCulture, format, args);

            if (exception != null)
                logEventInfo.Exception = new DetailedException(exception);
            _logger.Log(logEventInfo);
        }

        #endregion Implementation of ILogger
    }
}