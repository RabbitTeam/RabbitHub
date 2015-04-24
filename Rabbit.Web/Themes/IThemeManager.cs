using Rabbit.Kernel;
using Rabbit.Kernel.Extensions.Models;
using System.Web.Routing;

namespace Rabbit.Web.Themes
{
    /// <summary>
    /// 一个抽象的主题管理者。
    /// </summary>
    public interface IThemeManager : IDependency
    {
        /// <summary>
        /// 获取当前请求的主题。
        /// </summary>
        /// <param name="requestContext">请求上下文。</param>
        /// <returns>主题。</returns>
        ExtensionDescriptorEntry GetRequestTheme(RequestContext requestContext);
    }
}