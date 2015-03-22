using Rabbit.Kernel.Environment.Configuration;

namespace Rabbit.Components.Data.Utility.Extensions
{
    /// <summary>
    /// 外壳设置扩展。
    /// </summary>
    public static class ShellSettingsExtensions
    {
        /// <summary>
        /// 获取数据提供程序。
        /// </summary>
        /// <param name="shellSettings">外壳设置。</param>
        /// <returns>数据提供程序。</returns>
        public static string GetDataProvider(this ShellSettings shellSettings)
        {
            return shellSettings["DataProvider"] ?? string.Empty;
        }

        /// <summary>
        /// 获取数据连接字符串。
        /// </summary>
        /// <param name="shellSettings">外壳设置。</param>
        /// <returns>数据连接字符串。</returns>
        public static string GetDataConnectionString(this ShellSettings shellSettings)
        {
            return shellSettings["DataConnectionString"];
        }

        /// <summary>
        /// 获取数据表前缀。
        /// </summary>
        /// <param name="shellSettings">外壳设置。</param>
        /// <returns>数据表前缀。</returns>
        public static string GetDataTablePrefix(this ShellSettings shellSettings)
        {
            return shellSettings["DataTablePrefix"];
        }
    }
}