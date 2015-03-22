using Autofac;
using Rabbit.Kernel;
using Rabbit.Kernel.Environment.ShellBuilders;
using Rabbit.Kernel.Logging;

namespace Rabbit.Components.Logging.NLog
{
    /// <summary>
    ///     日志建设者扩展方法。
    /// </summary>
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        ///     使用NLog。
        /// </summary>
        /// <param name="loggingBuilder">日志建设者。</param>
        public static void UseNLog(this BuilderExtensions.ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.KernelBuilder
                .RegisterExtension(typeof(LoggingBuilderExtensions).Assembly)
                .OnStarting(builder =>
                {
                    builder.RegisterType<MinimumShellDescriptorProvider>()
                        .As<IMinimumShellDescriptorProvider>()
                        .SingleInstance();
                    builder.RegisterModule<LoggingModule>();
                });
        }
    }
}