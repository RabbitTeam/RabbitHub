using Rabbit.Components.Data.EntityFramework.Impl;
using Rabbit.Kernel;
using System.Data.Entity;

namespace Rabbit.Components.Data.EntityFramework
{
    /// <summary>
    ///     数据建设者扩展方法。
    /// </summary>
    public static class DataBuilderExtensions
    {
        /// <summary>
        ///     使用 Entity Framework 数据。
        /// </summary>
        /// <param name="dataBuilder">数据建设者。</param>
        public static void UseEntityFramework(this BuilderExtensions.IDataBuilder dataBuilder)
        {
            dataBuilder.KernelBuilder
                .RegisterExtension(typeof(DataBuilderExtensions).Assembly)
                .OnStarted(c => DbConfiguration.SetConfiguration(new DefaultDbConfiguration()));
        }
    }
}