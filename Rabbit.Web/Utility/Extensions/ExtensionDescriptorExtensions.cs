using Rabbit.Kernel.Extensions.Models;

namespace Rabbit.Web.Utility.Extensions
{
    /// <summary>
    /// 扩展描述符扩展方法。
    /// </summary>
    public static class ExtensionDescriptorExtensions
    {
        /// <summary>
        /// 获取基本主题。
        /// </summary>
        /// <param name="descriptor">扩展描述符。</param>
        /// <returns>基本主题。</returns>
        public static string GetBaseTheme(this ExtensionDescriptor descriptor)
        {
            return descriptor["BaseTheme"];
        }
    }
}