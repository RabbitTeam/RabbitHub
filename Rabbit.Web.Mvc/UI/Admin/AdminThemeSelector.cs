using Rabbit.Web.Themes;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.UI.Admin
{
    internal sealed class AdminThemeSelector : IThemeSelector
    {
        #region Implementation of IThemeSelector

        /// <summary>
        /// 根据当前请求获取主题。
        /// </summary>
        /// <param name="context">请求上下文。</param>
        /// <returns>主题选择结果。</returns>
        public ThemeSelectorResult GetTheme(RequestContext context)
        {
            return AdminFilter.IsApplied(context) ? new ThemeSelectorResult { Priority = 100, ThemeName = "TheAdmin" } : null;
        }

        #endregion Implementation of IThemeSelector
    }
}