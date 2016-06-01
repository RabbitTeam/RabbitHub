using Rabbit.Components.Data.EntityFramework.Impl;
using Rabbit.Kernel;
using System.Data.Entity;

namespace Rabbit.Components.Data.EntityFramework
{
    /// <summary>
    /// 数据建设者扩展方法。
    /// </summary>
    public static class DataBuilderExtensions
    {
        /// <summary>
        /// EntityFramework提供程序建设者.
        /// </summary>
        public class EntityFrameworkProviderBuilder
        {
            internal EntityFrameworkProviderBuilder(IKernelBuilder kernelBuilder)
            {
                KernelBuilder = kernelBuilder;
            }

            /// <summary>
            /// 内核建设者。
            /// </summary>
            public IKernelBuilder KernelBuilder { get; }
        }

        /// <summary>
        /// 使用 Entity Framework 数据。
        /// </summary>
        /// <param name="dataBuilder">数据建设者。</param>
        /// <returns>EntityFramework提供程序建设者</returns>
        public static EntityFrameworkProviderBuilder UseEntityFramework(this BuilderExtensions.IDataBuilder dataBuilder)
        {
            dataBuilder.KernelBuilder
                .RegisterExtension(typeof(DataBuilderExtensions).Assembly)
                .OnStarted(c => DbConfiguration.SetConfiguration(new DefaultDbConfiguration()));
            return new EntityFrameworkProviderBuilder(dataBuilder.KernelBuilder);
        }
    }
}