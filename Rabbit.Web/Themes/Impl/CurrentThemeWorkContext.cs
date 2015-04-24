using Rabbit.Kernel.Works;
using System;

namespace Rabbit.Web.Themes.Impl
{
    internal sealed class CurrentThemeWorkContext : IWorkContextStateProvider
    {
        #region Field

        private readonly IThemeManager _themeManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion Field

        #region Constructor

        public CurrentThemeWorkContext(IThemeManager themeManager, IHttpContextAccessor httpContextAccessor)
        {
            _themeManager = themeManager;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion Constructor

        #region Implementation of IWorkContextStateProvider

        /// <summary>
        /// 创建一个从一个工作上下文获取服务的委托。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <param name="name">服务名称。</param>
        /// <returns>从一个工作上下文获取服务的委托。</returns>
        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name != "CurrentTheme")
                return null;

            var currentTheme = _themeManager.GetRequestTheme(_httpContextAccessor.Current().Request.RequestContext);
            return ctx => (T)(object)currentTheme;
        }

        #endregion Implementation of IWorkContextStateProvider
    }
}