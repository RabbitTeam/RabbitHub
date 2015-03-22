using System;

namespace Rabbit.Kernel.Logging
{
    /// <summary>
    /// 日志建设者扩展。
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// 使用日志。
        /// </summary>
        /// <param name="kernelBuilder">内核建设者。</param>
        /// <param name="loggingBuilder">日志建设动作。</param>
        public static void UseLogging(this IKernelBuilder kernelBuilder, Action<ILoggingBuilder> loggingBuilder)
        {
            loggingBuilder(new LoggingBuilder(kernelBuilder));
        }

        /// <summary>
        /// 一个抽象的日志组件建设者。
        /// </summary>
        public interface ILoggingBuilder
        {
            /// <summary>
            /// 内核建设者。
            /// </summary>
            IKernelBuilder KernelBuilder { get; }
        }

        internal sealed class LoggingBuilder : ILoggingBuilder
        {
            #region Constructor

            public LoggingBuilder(IKernelBuilder kernelBuilder)
            {
                KernelBuilder = kernelBuilder;
            }

            #endregion Constructor

            #region Implementation of ILoggingBuilder

            /// <summary>
            /// 内核建设者。
            /// </summary>
            public IKernelBuilder KernelBuilder { get; private set; }

            #endregion Implementation of ILoggingBuilder
        }
    }
}