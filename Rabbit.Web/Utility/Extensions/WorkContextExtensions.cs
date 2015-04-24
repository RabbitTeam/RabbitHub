using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.Works;

namespace Rabbit.Web.Utility.Extensions
{
    /// <summary>
    /// 工作上下文扩展方法。
    /// </summary>
    public static class WorkContextExtensions
    {
        /// <summary>
        /// 获取当前主题。
        /// </summary>
        /// <param name="workContext">工作上下文。</param>
        /// <returns>主题扩展描述符条目。</returns>
        public static ExtensionDescriptorEntry GetCurrentTheme(this WorkContext workContext)
        {
            return workContext.GetState<ExtensionDescriptorEntry>("CurrentTheme");
        }
    }
}