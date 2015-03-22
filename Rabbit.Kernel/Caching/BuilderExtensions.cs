using Autofac;
using Rabbit.Kernel.Caching.Impl;
using System;

namespace Rabbit.Kernel.Caching
{
    /// <summary>
    /// 缓存建设者扩展。
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// 使用缓存。
        /// </summary>
        /// <param name="kernelBuilder">内核建设者。</param>
        /// <param name="cachingBuilder">缓存建设动作。</param>
        public static void UseCaching(this IKernelBuilder kernelBuilder, Action<ICachingBuilder> cachingBuilder)
        {
            kernelBuilder.OnStarting(builder =>
            {
                builder.RegisterType<DefaultCacheContextAccessor>().As<ICacheContextAccessor>().SingleInstance();
                builder.RegisterType<DefaultParallelCacheContext>().As<IParallelCacheContext>().SingleInstance();
                builder.RegisterType<DefaultAsyncTokenProvider>().As<IAsyncTokenProvider>().SingleInstance();
            });
            cachingBuilder(new CachingBuilder(kernelBuilder));
        }

        /// <summary>
        /// 一个抽象的缓存组件建设者。
        /// </summary>
        public interface ICachingBuilder
        {
            /// <summary>
            /// 内核建设者。
            /// </summary>
            IKernelBuilder KernelBuilder { get; }
        }

        internal sealed class CachingBuilder : ICachingBuilder
        {
            #region Constructor

            public CachingBuilder(IKernelBuilder kernelBuilder)
            {
                KernelBuilder = kernelBuilder;
            }

            #endregion Constructor

            #region Implementation of ICachingBuilder

            /// <summary>
            /// 内核建设者。
            /// </summary>
            public IKernelBuilder KernelBuilder { get; private set; }

            #endregion Implementation of ICachingBuilder
        }
    }
}