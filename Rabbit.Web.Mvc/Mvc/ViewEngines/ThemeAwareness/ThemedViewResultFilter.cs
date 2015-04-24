using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Works;
using Rabbit.Web.Mvc.Works;
using Rabbit.Web.Themes;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.ThemeAwareness
{
    internal sealed class ThemedViewResultFilter : Filters.IFilterProvider, IResultFilter
    {
        #region Field

        private readonly IThemeManager _themeManager;
        private readonly MvcWorkContext _workContext;
        private readonly ILayoutAwareViewEngine _layoutAwareViewEngine;

        #endregion Field

        #region Constructor

        public ThemedViewResultFilter(IThemeManager themeManager, WorkContext workContext, ILayoutAwareViewEngine layoutAwareViewEngine)
        {
            _themeManager = themeManager;
            _workContext = workContext.AsMvcWorkContext();
            _layoutAwareViewEngine = layoutAwareViewEngine;
            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Implementation of IResultFilter

        /// <summary>
        /// 在操作结果执行之前调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var viewResultBase = filterContext.Result as ViewResultBase;
            if (viewResultBase == null)
            {
                return;
            }

            if (_workContext.CurrentTheme == null)
            {
                _workContext.CurrentTheme = _themeManager.GetRequestTheme(filterContext.RequestContext);
            }

            viewResultBase.ViewEngineCollection = new ViewEngineCollection(new IViewEngine[] { _layoutAwareViewEngine });
        }

        /// <summary>
        /// 在操作结果执行后调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        #endregion Implementation of IResultFilter
    }
}