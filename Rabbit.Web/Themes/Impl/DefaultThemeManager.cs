using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace Rabbit.Web.Themes.Impl
{
    internal sealed class DefaultThemeManager : IThemeManager
    {
        #region Field

        private readonly IEnumerable<IThemeSelector> _themeSelectors;
        private readonly IExtensionManager _extensionManager;

        #endregion Field

        #region Constructor

        public DefaultThemeManager(IEnumerable<IThemeSelector> themeSelectors,
                            IExtensionManager extensionManager)
        {
            _themeSelectors = themeSelectors;
            _extensionManager = extensionManager;
        }

        #endregion Constructor

        #region Implementation of IThemeManager

        /// <summary>
        /// 获取当前请求的主题。
        /// </summary>
        /// <param name="requestContext">请求上下文。</param>
        /// <returns>主题。</returns>
        public ExtensionDescriptorEntry GetRequestTheme(RequestContext requestContext)
        {
            var requestTheme = _themeSelectors
                .Select(x => x.GetTheme(requestContext))
                .Where(x => x != null)
                .OrderByDescending(x => x.Priority);

            if (!requestTheme.Any())
                return null;

            var theme =
                requestTheme.Select(t => _extensionManager.GetExtension(t.ThemeName)).FirstOrDefault(t => t != null);

            return theme ?? _extensionManager.GetExtension("SafeMode");
        }

        #endregion Implementation of IThemeManager
    }
}