using NLog;

namespace Rabbit.Components.Logging.NLog
{
    internal static class LogUtilities
    {
        public static LogLevel ConvertLogLevel(Kernel.Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Kernel.Logging.LogLevel.Debug:
                    return LogLevel.Debug;

                case Kernel.Logging.LogLevel.Error:
                    return LogLevel.Error;

                case Kernel.Logging.LogLevel.Fatal:
                    return LogLevel.Fatal;

                case Kernel.Logging.LogLevel.Information:
                    return LogLevel.Info;

                case Kernel.Logging.LogLevel.Trace:
                    return LogLevel.Trace;

                case Kernel.Logging.LogLevel.Warning:
                    return LogLevel.Warn;

                default:
                    return LogLevel.Off;
            }
        }
    }
}