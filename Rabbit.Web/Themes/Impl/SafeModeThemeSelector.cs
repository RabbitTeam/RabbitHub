using System.Web.Routing;

namespace Rabbit.Web.Themes.Impl
{
    internal sealed class SafeModeThemeSelector : IThemeSelector
    {
        #region Implementation of IThemeSelector

        /// <summary>
        /// 根据当前请求获取主题。
        /// </summary>
        /// <param name="context">请求上下文。</param>
        /// <returns>主题选择结果。</returns>
        public ThemeSelectorResult GetTheme(RequestContext context)
        {
            return new ThemeSelectorResult { Priority = -100, ThemeName = "Themes" };
        }

        #endregion Implementation of IThemeSelector
    }
}