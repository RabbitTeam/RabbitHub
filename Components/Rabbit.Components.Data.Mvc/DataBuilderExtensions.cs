using Rabbit.Kernel;

namespace Rabbit.Components.Data.Mvc
{
    /// <summary>
    /// 数据建设者。
    /// </summary>
    public static class DataBuilderExtensions
    {
        /// <summary>
        /// 启用Mvc拦截器事物支持，当Action执行失败时执行事务回滚。
        /// </summary>
        /// <param name="dataBuilder">数据建设者。</param>
        public static void EnableMvcFilterTransaction(this BuilderExtensions.IDataBuilder dataBuilder)
        {
            dataBuilder.KernelBuilder.RegisterExtension(typeof(DataBuilderExtensions).Assembly);
        }
    }
}