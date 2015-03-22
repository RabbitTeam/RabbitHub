using Autofac;

namespace Rabbit.Kernel.Caching.Impl
{
    /// <summary>
    /// 缓存建设者扩展方法。
    /// </summary>
    public static class MemoryCachingBuilderExtensions
    {
        /// <summary>
        /// 使用内存缓存。
        /// </summary>
        /// <param name="cachingBuilder">缓存建设者。</param>
        public static void UseMemoryCache(this BuilderExtensions.ICachingBuilder cachingBuilder)
        {
            cachingBuilder.KernelBuilder
                .RegisterExtension(typeof(MemoryCachingBuilderExtensions).Assembly)
                .OnStarting(builder => builder.RegisterType<DefaultCacheHolder>().As<ICacheHolder>().SingleInstance());
        }
    }
}