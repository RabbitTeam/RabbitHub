using Rabbit.Kernel;

namespace Rabbit.Components.Data.Migrators
{
    /// <summary>
    /// 数据建设者扩展方法。
    /// </summary>
    public static class DataMigratorsBuilderExtensions
    {
        /// <summary>
        /// 启用数据迁移。
        /// </summary>
        /// <param name="dataBuilder">数据建设者。</param>
        public static void EnableDataMigrators(this BuilderExtensions.IDataBuilder dataBuilder)
        {
            dataBuilder.KernelBuilder.RegisterExtension(typeof(DataMigratorsBuilderExtensions).Assembly);
        }
    }
}