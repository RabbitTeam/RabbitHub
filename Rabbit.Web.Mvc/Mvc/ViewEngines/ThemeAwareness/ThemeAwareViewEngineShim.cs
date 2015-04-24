using Rabbit.Kernel.Environment;
using Rabbit.Web.Mvc.Utility.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.ThemeAwareness
{
    internal class ThemeAwareViewEngineShim : IViewEngine, IShim
    {
        public ThemeAwareViewEngineShim()
        {
            HostContainerRegistry.RegisterShim(this);
        }

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
            return Forward(
                controllerContext,
                dve => dve.FindPartialView(controllerContext, partialViewName, useCache, false),
                EmptyViewEngineResult);
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
            return Forward(
                controllerContext,
                dve => dve.FindView(controllerContext, viewName, masterName, useCache, false),
                EmptyViewEngineResult);
        }

        /// <summary>
        /// 使用指定的控制器上下文来释放指定的视图。
        /// </summary>
        /// <param name="controllerContext">控制器上下文。</param><param name="view">视图。</param>
        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            throw new NotImplementedException();
        }

        #endregion Implementation of IViewEngine

        #region Implementation of IShim

        /// <summary>
        /// 主机容器。
        /// </summary>
        public IHostContainer HostContainer { get; set; }

        #endregion Implementation of IShim

        #region Private Method

        private static TResult Forward<TResult>(ControllerContext controllerContext, Func<IThemeAwareViewEngine, TResult> forwardAction, Func<TResult> defaultAction)
        {
            var workContext = controllerContext.GetWorkContext();
            if (workContext != null)
            {
                var displayViewEngine = workContext.Resolve<IThemeAwareViewEngine>();
                if (displayViewEngine != null)
                {
                    return forwardAction(displayViewEngine);
                }
            }
            return defaultAction();
        }

        private static ViewEngineResult EmptyViewEngineResult()
        {
            return new ViewEngineResult(Enumerable.Empty<string>());
        }

        #endregion Private Method
    }
}