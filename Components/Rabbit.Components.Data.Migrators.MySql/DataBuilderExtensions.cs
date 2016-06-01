using Rabbit.Kernel;

namespace Rabbit.Components.Data.Migrators.MySql
{
    /// <summary>
    /// 数据建设者扩展方法。
    /// </summary>
    public static class DataBuilderExtensions
    {
        /// <summary>
        /// 添加MySql迁移的支持程序。
        /// </summary>
        /// <param name="dataBuilder">数据建设者。</param>
        /// <returns>数据建设者。</returns>
        public static BuilderExtensions.IDataBuilder AddMySqlMigratorProvider(this BuilderExtensions.IDataBuilder dataBuilder)
        {
            dataBuilder.KernelBuilder
                .RegisterExtension(typeof(DataBuilderExtensions).Assembly);
            return dataBuilder;
        }
    }
}