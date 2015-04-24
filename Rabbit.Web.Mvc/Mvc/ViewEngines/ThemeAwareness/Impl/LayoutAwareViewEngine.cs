using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Works;
using Rabbit.Web.Mvc.DisplayManagement;
using Rabbit.Web.Mvc.Mvc.Spooling;
using Rabbit.Web.Mvc.Themes;
using Rabbit.Web.Mvc.Works;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.ThemeAwareness.Impl
{
    internal sealed class LayoutAwareViewEngine : ILayoutAwareViewEngine
    {
        #region Field

        private readonly IThemeAwareViewEngine _themeAwareViewEngine;
        private readonly MvcWorkContext _workContext;
        private readonly IDisplayHelperFactory _displayHelperFactory;

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Constructor

        public LayoutAwareViewEngine(IThemeAwareViewEngine themeAwareViewEngine, WorkContext workContext, IDisplayHelperFactory displayHelperFactory)
        {
            _themeAwareViewEngine = themeAwareViewEngine;
            _workContext = workContext.AsMvcWorkContext();
            _displayHelperFactory = displayHelperFactory;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IViewEngine

        /// <summary>
        /// 使用指定的控制器上下文查找指定的分部视图。
        /// </summary>
        /// <returns>
        /// 分部视图。
        /// </returns>
        /// <param name="controllerContext">控制器上下文。</param><param name="partialViewName">分部视图的名称。</param><param name="useCache">若指定视图引擎返回缓存的视图（如果存在缓存的视图），则为 true；否则为 false。</param>
        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return _themeAwareViewEngine.FindPartialView(controllerContext, partialViewName, useCache, true);
        }

        /// <summary>
        /// 使用指定的控制器上下文来查找指定的视图。
        /// </summary>
        /// <returns>
        /// 页视图。
        /// </returns>
        /// <param name="controllerContext">控制器上下文。</param><param name="viewName">视图的名称。</param><param name="masterName">母版的名称。</param><param name="useCache">若指定视图引擎返回缓存的视图（如果存在缓存的视图），则为 true；否则为 false。</param>
        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var viewResult = _themeAwareViewEngine.FindPartialView(controllerContext, viewName, useCache, true);

            if (viewResult.View == null)
            {
                return viewResult;
            }

            if (!ThemeFilter.IsApplied(controllerContext.RequestContext))
            {
                return viewResult;
            }

            var layoutView = new LayoutView((viewContext, writer, viewDataContainer) =>
            {
                Logger.Information("准备呈现布局视图");

                var childContentWriter = new HtmlStringWriter();

                var childContentViewContext = new ViewContext(
                    viewContext,
                    viewContext.View,
                    viewContext.ViewData,
                    viewContext.TempData,
                    childContentWriter);

                viewResult.View.Render(childContentViewContext, childContentWriter);
                _workContext.Layout.Metadata.ChildContent = childContentWriter;

                var display = _displayHelperFactory.CreateHelper(viewContext, viewDataContainer);
                IHtmlString result = display(_workContext.Layout);
                writer.Write(result.ToHtmlString());

                Logger.Information("完成布局视图的呈现");
            }, (context, view) => viewResult.ViewEngine.ReleaseView(context, viewResult.View));

            return new ViewEngineResult(layoutView, this);
        }

        /// <summary>
        /// 使用指定的控制器上下文来释放指定的视图。
        /// </summary>
        /// <param name="controllerContext">控制器上下文。</param><param name="view">视图。</param>
        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            var layoutView = (LayoutView)view;
            layoutView.ReleaseView(controllerContext, view);
        }

        #endregion Implementation of IViewEngine

        #region Help Class

        private sealed class LayoutView : IView, IViewDataContainer
        {
            private readonly Action<ViewContext, TextWriter, IViewDataContainer> _render;
            private readonly Action<ControllerContext, IView> _releaseView;

            public LayoutView(Action<ViewContext, TextWriter, IViewDataContainer> render, Action<ControllerContext, IView> releaseView)
            {
                _render = render;
                _releaseView = releaseView;
            }

            public ViewDataDictionary ViewData { get; set; }

            public void Render(ViewContext viewContext, TextWriter writer)
            {
                ViewData = viewContext.ViewData;
                _render(viewContext, writer, this);
            }

            public void ReleaseView(ControllerContext context, IView view)
            {
                _releaseView(context, view);
            }
        }

        #endregion Help Class
    }
}