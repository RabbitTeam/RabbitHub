using Autofac;
using Rabbit.Kernel;
using Rabbit.Kernel.Environment.ShellBuilders;
using System;

namespace Rabbit.Components.Data
{
    /// <summary>
    /// 建设者扩展方法。
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// 使用数据。
        /// </summary>
        /// <param name="kernelBuilder">内核建设者。</param>
        /// <param name="dataBuilder">数据建设者。</param>
        public static void UseData(this IKernelBuilder kernelBuilder, Action<IDataBuilder> dataBuilder)
        {
            kernelBuilder.OnStarting(builder => builder.RegisterType<CompositionStrategyProvider>().As<ICompositionStrategyProvider>().SingleInstance());
            if (dataBuilder != null)
                dataBuilder(new DataBuilder(kernelBuilder));
        }

        /// <summary>
        /// 一个抽象的数据建设者。
        /// </summary>
        public interface IDataBuilder
        {
            /// <summary>
            /// 内核建设者。
            /// </summary>
            IKernelBuilder KernelBuilder { get; }
        }

        private sealed class DataBuilder : IDataBuilder
        {
            public DataBuilder(IKernelBuilder kernelBuilder)
            {
                KernelBuilder = kernelBuilder;
            }

            #region Implementation of IDataBuilder

            public IKernelBuilder KernelBuilder { get; private set; }

            #endregion Implementation of IDataBuilder
        }
    }
}