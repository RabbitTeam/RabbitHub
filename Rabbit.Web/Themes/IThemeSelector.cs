using Rabbit.Kernel;
using System.Web.Routing;

namespace Rabbit.Web.Themes
{
    /// <summary>
    /// 主题选择结果。
    /// </summary>
    public class ThemeSelectorResult
    {
        /// <summary>
        /// 优先级。
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 主题名称。
        /// </summary>
        public string ThemeName { get; set; }
    }

    /// <summary>
    /// 一个抽象的主题选择器。
    /// </summary>
    public interface IThemeSelector : IDependency
    {
        /// <summary>
        /// 根据当前请求获取主题。
        /// </summary>
        /// <param name="context">请求上下文。</param>
        /// <returns>主题选择结果。</returns>
        ThemeSelectorResult GetTheme(RequestContext context);
    }
}