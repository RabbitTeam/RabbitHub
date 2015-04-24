using Rabbit.Kernel.Works;
using Rabbit.Web.Works;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Works
{
    /// <summary>
    /// 工作上下文扩展方法。
    /// </summary>
    public static class WorkContextExtensions
    {
        /// <summary>
        /// 获取工作上下文。
        /// </summary>
        /// <param name="workContextAccessor">工作上下文访问器。</param>
        /// <param name="controllerContext">控制器上下文。</param>
        /// <returns>工作上下文。</returns>
        public static WorkContext GetWorkContext(this IWebWorkContextAccessor workContextAccessor, ControllerContext controllerContext)
        {
            return workContextAccessor.GetContext(controllerContext.RequestContext.HttpContext);
        }

        /// <summary>
        /// 将工作上下文转换为Mvc工作上下文。
        /// </summary>
        /// <param name="workContext">工作上下文。</param>
        /// <returns>Mvc工作上下文。</returns>
        public static MvcWorkContext AsMvcWorkContext(this WorkContext workContext)
        {
            if (workContext == null)
                return null;
            var work = workContext.GetState<MvcWorkContext>("WebWorkContext");
            if (work != null)
                return work;
            work = new MvcWorkContext(workContext);
            workContext.SetState("WebWorkContext", work);
            return work;
        }

        /// <summary>
        /// 获取工作上下文。
        /// </summary>
        /// <param name="controllerContext">控制器上下文。</param>
        /// <returns>工作上下文。</returns>
        public static WorkContext GetWorkContext(this HttpControllerContext controllerContext)
        {
            if (controllerContext == null)
                return null;

            var routeData = controllerContext.RouteData;
            if (routeData == null || routeData.Values == null)
                return null;

            object workContextValue;
            if (!routeData.Values.TryGetValue("IWorkContextAccessor", out workContextValue))
            {
                return null;
            }

            if (!(workContextValue is IWorkContextAccessor))
                return null;

            var workContextAccessor = (IWorkContextAccessor)workContextValue;
            return workContextAccessor.GetContext();
        }
    }
}