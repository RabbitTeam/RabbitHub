using Rabbit.Kernel.Works;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web.Works
{
    /// <summary>
    /// 工作上下文扩展方法。
    /// </summary>
    public static class WorkContextExtensions
    {
        /// <summary>
        /// 将工作上下文转换为Web工作上下文。
        /// </summary>
        /// <param name="workContext">工作上下文。</param>
        /// <returns>Web工作上下文。</returns>
        public static WebWorkContext AsWebWorkContext(this WorkContext workContext)
        {
            var work = workContext.GetState<WebWorkContext>("WebWorkContext");
            if (work != null)
                return work;
            work = new WebWorkContext(workContext);
            workContext.SetState("WebWorkContext", work);
            return work;
        }

        /// <summary>
        /// 获取工作上下文。
        /// </summary>
        /// <param name="requestContext">请求上下文。</param>
        /// <returns>工作上下文。</returns>
        public static WorkContext GetWorkContext(this RequestContext requestContext)
        {
            if (requestContext == null)
                return null;

            var routeData = requestContext.RouteData;
            if (routeData == null || routeData.DataTokens == null)
                return null;

            object workContextValue;
            if (!routeData.DataTokens.TryGetValue("IWorkContextAccessor", out workContextValue))
            {
                workContextValue = FindWorkContextInParent(routeData);
            }

            if (!(workContextValue is IWebWorkContextAccessor))
                return null;

            var workContextAccessor = workContextValue as IWebWorkContextAccessor;
            return workContextAccessor.GetContext(requestContext.HttpContext);
        }

        #region Private Method

        private static object FindWorkContextInParent(RouteData routeData)
        {
            object parentViewContextValue;
            if (!routeData.DataTokens.TryGetValue("ParentActionViewContext", out parentViewContextValue)
                || !(parentViewContextValue is ViewContext))
            {
                return null;
            }

            var parentRouteData = ((ViewContext)parentViewContextValue).RouteData;
            if (parentRouteData == null || parentRouteData.DataTokens == null)
                return null;

            object workContextValue;
            if (!parentRouteData.DataTokens.TryGetValue("IWorkContextAccessor", out workContextValue))
            {
                workContextValue = FindWorkContextInParent(parentRouteData);
            }

            return workContextValue;
        }

        #endregion Private Method
    }
}