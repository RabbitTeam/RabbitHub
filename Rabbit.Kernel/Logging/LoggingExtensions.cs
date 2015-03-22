using Rabbit.Kernel.Localization;
using System;

namespace Rabbit.Kernel.Logging
{
    /// <summary>
    /// 日志记录器扩展方法。
    /// </summary>
    public static class LoggingExtenions
    {
        #region Public Method

        /// <summary>
        /// 记录一个调试信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Debug(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Debug, null, message, null);
        }

        /// <summary>
        /// 记录一个信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Information(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Information, null, message, null);
        }

        /// <summary>
        /// 记录一个警告信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Warning(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Warning, null, message, null);
        }

        /// <summary>
        /// 记录一个错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Error(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Error, null, message, null);
        }

        /// <summary>
        /// 记录一个致命的错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Fatal(this ILogger logger, string message)
        {
            FilteredLog(logger, LogLevel.Fatal, null, message, null);
        }

        /// <summary>
        /// 记录一个调试信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Debug(this ILogger logger, Exception exception, string message)
        {
            FilteredLog(logger, LogLevel.Debug, exception, message, null);
        }

        /// <summary>
        /// 记录一个信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Information(this ILogger logger, Exception exception, string message)
        {
            FilteredLog(logger, LogLevel.Information, exception, message, null);
        }

        /// <summary>
        /// 记录一个警告信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Warning(this ILogger logger, Exception exception, string message)
        {
            FilteredLog(logger, LogLevel.Warning, exception, message, null);
        }

        /// <summary>
        /// 记录一个错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Error(this ILogger logger, Exception exception, string message)
        {
            FilteredLog(logger, LogLevel.Error, exception, message, null);
        }

        /// <summary>
        /// 记录一个致命的错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Fatal(this ILogger logger, Exception exception, string message)
        {
            FilteredLog(logger, LogLevel.Fatal, exception, message, null);
        }

        /// <summary>
        /// 记录一个调试信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Debug(this ILogger logger, Func<string> message)
        {
            FilteredLog(logger, LogLevel.Debug, null, message);
        }

        /// <summary>
        /// 记录一个信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Information(this ILogger logger, Func<string> message)
        {
            FilteredLog(logger, LogLevel.Information, null, message);
        }

        /// <summary>
        /// 记录一个警告信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Warning(this ILogger logger, Func<string> message)
        {
            FilteredLog(logger, LogLevel.Warning, null, message);
        }

        /// <summary>
        /// 记录一个错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Error(this ILogger logger, Func<string> message)
        {
            FilteredLog(logger, LogLevel.Error, null, message);
        }

        /// <summary>
        /// 记录一个致命的错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Fatal(this ILogger logger, Func<string> message)
        {
            FilteredLog(logger, LogLevel.Fatal, null, message);
        }

        /// <summary>
        /// 记录一个调试信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Debug(this ILogger logger, Exception exception, Func<string> message)
        {
            FilteredLog(logger, LogLevel.Debug, exception, message);
        }

        /// <summary>
        /// 记录一个信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Information(this ILogger logger, Exception exception, Func<string> message)
        {
            FilteredLog(logger, LogLevel.Information, exception, message);
        }

        /// <summary>
        /// 记录一个警告信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Warning(this ILogger logger, Exception exception, Func<string> message)
        {
            FilteredLog(logger, LogLevel.Warning, exception, message);
        }

        /// <summary>
        /// 记录一个错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Error(this ILogger logger, Exception exception, Func<string> message)
        {
            FilteredLog(logger, LogLevel.Error, exception, message);
        }

        /// <summary>
        /// 记录一个致命的错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Fatal(this ILogger logger, Exception exception, Func<string> message)
        {
            FilteredLog(logger, LogLevel.Fatal, exception, message);
        }

        /// <summary>
        /// 记录一个调试信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Debug(this ILogger logger, Func<Exception> exception, Func<string> message)
        {
            FilteredLogFuncException(logger, LogLevel.Debug, exception, message);
        }

        /// <summary>
        /// 记录一个信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Information(this ILogger logger, Func<Exception> exception, Func<string> message)
        {
            FilteredLogFuncException(logger, LogLevel.Information, exception, message);
        }

        /// <summary>
        /// 记录一个警告信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Warning(this ILogger logger, Func<Exception> exception, Func<string> message)
        {
            FilteredLogFuncException(logger, LogLevel.Warning, exception, message);
        }

        /// <summary>
        /// 记录一个错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Error(this ILogger logger, Func<Exception> exception, Func<string> message)
        {
            FilteredLogFuncException(logger, LogLevel.Error, exception, message);
        }

        /// <summary>
        /// 记录一个致命的错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Fatal(this ILogger logger, Func<Exception> exception, Func<string> message)
        {
            FilteredLogFuncException(logger, LogLevel.Fatal, exception, message);
        }

        /// <summary>
        /// 记录一个调试信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Debug(this ILogger logger, string format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Debug, null, format, args);
        }

        /// <summary>
        /// 记录一个信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Information(this ILogger logger, string format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Information, null, format, args);
        }

        /// <summary>
        /// 记录一个警告信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Warning(this ILogger logger, string format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Warning, null, format, args);
        }

        /// <summary>
        /// 记录一个错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Error(this ILogger logger, string format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Error, null, format, args);
        }

        /// <summary>
        /// 记录一个致命的错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Fatal(this ILogger logger, string format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Fatal, null, format, args);
        }

        /// <summary>
        /// 记录一个调试信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Debug(this ILogger logger, Exception exception, string format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Debug, exception, format, args);
        }

        /// <summary>
        /// 记录一个信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Information(this ILogger logger, Exception exception, string format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Information, exception, format, args);
        }

        /// <summary>
        /// 记录一个警告信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Warning(this ILogger logger, Exception exception, string format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Warning, exception, format, args);
        }

        /// <summary>
        /// 记录一个错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Error(this ILogger logger, Exception exception, string format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Error, exception, format, args);
        }

        /// <summary>
        /// 记录一个致命的错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Fatal(this ILogger logger, Exception exception, string format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Fatal, exception, format, args);
        }

        /// <summary>
        /// 记录一个调试信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Debug(this ILogger logger, LocalizedString message)
        {
            FilteredLog(logger, LogLevel.Debug, null, message.ToString(), null);
        }

        /// <summary>
        /// 记录一个信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Information(this ILogger logger, LocalizedString message)
        {
            FilteredLog(logger, LogLevel.Information, null, message.ToString(), null);
        }

        /// <summary>
        /// 记录一个警告信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Warning(this ILogger logger, LocalizedString message)
        {
            FilteredLog(logger, LogLevel.Warning, null, message.ToString(), null);
        }

        /// <summary>
        /// 记录一个错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Error(this ILogger logger, LocalizedString message)
        {
            FilteredLog(logger, LogLevel.Error, null, message.ToString(), null);
        }

        /// <summary>
        /// 记录一个致命的错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="message">消息。</param>
        public static void Fatal(this ILogger logger, LocalizedString message)
        {
            FilteredLog(logger, LogLevel.Fatal, null, message.ToString(), null);
        }

        /// <summary>
        /// 记录一个调试信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Debug(this ILogger logger, Exception exception, LocalizedString message)
        {
            FilteredLog(logger, LogLevel.Debug, exception, message.ToString(), null);
        }

        /// <summary>
        /// 记录一个信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Information(this ILogger logger, Exception exception, LocalizedString message)
        {
            FilteredLog(logger, LogLevel.Information, exception, message.ToString(), null);
        }

        /// <summary>
        /// 记录一个警告信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Warning(this ILogger logger, Exception exception, LocalizedString message)
        {
            FilteredLog(logger, LogLevel.Warning, exception, message.ToString(), null);
        }

        /// <summary>
        /// 记录一个错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Error(this ILogger logger, Exception exception, LocalizedString message)
        {
            FilteredLog(logger, LogLevel.Error, exception, message.ToString(), null);
        }

        /// <summary>
        /// 记录一个致命的错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="message">消息。</param>
        public static void Fatal(this ILogger logger, Exception exception, LocalizedString message)
        {
            FilteredLog(logger, LogLevel.Fatal, exception, message.ToString(), null);
        }

        /// <summary>
        /// 记录一个调试信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Debug(this ILogger logger, LocalizedString format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Debug, null, format.ToString(), args);
        }

        /// <summary>
        /// 记录一个信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Information(this ILogger logger, LocalizedString format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Information, null, format.ToString(), args);
        }

        /// <summary>
        /// 记录一个警告信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Warning(this ILogger logger, LocalizedString format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Warning, null, format.ToString(), args);
        }

        /// <summary>
        /// 记录一个错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Error(this ILogger logger, LocalizedString format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Error, null, format.ToString(), args);
        }

        /// <summary>
        /// 记录一个致命的错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Fatal(this ILogger logger, LocalizedString format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Fatal, null, format.ToString(), args);
        }

        /// <summary>
        /// 记录一个调试信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Debug(this ILogger logger, Exception exception, LocalizedString format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Debug, exception, format.ToString(), args);
        }

        /// <summary>
        /// 记录一个信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Information(this ILogger logger, Exception exception, LocalizedString format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Information, exception, format.ToString(), args);
        }

        /// <summary>
        /// 记录一个警告信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Warning(this ILogger logger, Exception exception, LocalizedString format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Warning, exception, format.ToString(), args);
        }

        /// <summary>
        /// 记录一个错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Error(this ILogger logger, Exception exception, LocalizedString format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Error, exception, format.ToString(), args);
        }

        /// <summary>
        /// 记录一个致命的错误信息。
        /// </summary>
        /// <param name="logger">日志记录器。</param>
        /// <param name="exception">异常。</param>
        /// <param name="format">记录格式。</param>
        /// <param name="args">参数。</param>
        public static void Fatal(this ILogger logger, Exception exception, LocalizedString format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Fatal, exception, format.ToString(), args);
        }

        #endregion Public Method

        #region Private Method

        private static void FilteredLogFuncException(ILogger logger, LogLevel level, Func<Exception> exception, Func<string> message)
        {
            if (logger.IsEnabled(level))
                logger.Log(level, exception == null ? null : exception(), message == null ? null : message());
        }

        private static void FilteredLog(ILogger logger, LogLevel level, Exception exception, Func<string> message)
        {
            if (logger.IsEnabled(level))
                logger.Log(level, exception, message == null ? null : message());
        }

        private static void FilteredLog(ILogger logger, LogLevel level, Exception exception, string format, object[] objects)
        {
            if (logger.IsEnabled(level))
            {
                logger.Log(level, exception, format, objects);
            }
        }

        #endregion Private Method
    }
}