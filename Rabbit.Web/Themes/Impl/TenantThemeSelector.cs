using Rabbit.Kernel.Environment.Configuration;
using System.Web.Routing;

namespace Rabbit.Web.Themes.Impl
{
    internal sealed class TenantThemeSelector : IThemeSelector
    {
        #region Field

        private readonly ShellSettings _settings;

        #endregion Field

        #region Constructor

        public TenantThemeSelector(ShellSettings settings)
        {
            _settings = settings;
        }

        #endregion Constructor

        #region Implementation of IThemeSelector

        /// <summary>
        /// 根据当前请求获取主题。
        /// </summary>
        /// <param name="context">请求上下文。</param>
        /// <returns>主题选择结果。</returns>
        public ThemeSelectorResult GetTheme(RequestContext context)
        {
            return new ThemeSelectorResult { Priority = -5, ThemeName = string.Format("{0}_TheThemeMachine", _settings.Name) };
        }

        #endregion Implementation of IThemeSelector
    }
}