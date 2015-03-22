using System;

namespace Rabbit.Kernel.Bus
{
    /// <summary>
    /// 总线建设者扩展。
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// 使用总线。
        /// </summary>
        /// <param name="kernelBuilder">内核建设者。</param>
        /// <param name="busBuilder">总线建设动作。</param>
        public static void UseBus(this IKernelBuilder kernelBuilder, Action<IBusBuilder> busBuilder)
        {
            busBuilder(new BusBuilder(kernelBuilder));
        }

        /// <summary>
        /// 一个抽象的总线组件建设者。
        /// </summary>
        public interface IBusBuilder
        {
            /// <summary>
            /// 内核建设者。
            /// </summary>
            IKernelBuilder KernelBuilder { get; }
        }

        internal sealed class BusBuilder : IBusBuilder
        {
            #region Constructor

            public BusBuilder(IKernelBuilder kernelBuilder)
            {
                KernelBuilder = kernelBuilder;
            }

            #endregion Constructor

            #region Implementation of IBusBuilder

            /// <summary>
            /// 内核建设者。
            /// </summary>
            public IKernelBuilder KernelBuilder { get; private set; }

            #endregion Implementation of IBusBuilder
        }
    }
}