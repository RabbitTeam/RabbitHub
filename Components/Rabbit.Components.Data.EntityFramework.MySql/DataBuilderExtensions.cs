using Rabbit.Kernel;

namespace Rabbit.Components.Data.EntityFramework.MySql
{
    /// <summary>
    /// 数据建设者扩展方法。
    /// </summary>
    public static class DataBuilderExtensions
    {
        /// <summary>
        /// 添加MySql迁移的支持程序。
        /// </summary>
        /// <param name="builder">EntityFramework提供程序建设者。</param>
        /// <returns>EntityFramework提供程序建设者。</returns>
        public static EntityFramework.DataBuilderExtensions.EntityFrameworkProviderBuilder AddMySql(this EntityFramework.DataBuilderExtensions.EntityFrameworkProviderBuilder builder)
        {
            builder.KernelBuilder
                .RegisterExtension(typeof(DataBuilderExtensions).Assembly);
            return builder;
        }
    }
}