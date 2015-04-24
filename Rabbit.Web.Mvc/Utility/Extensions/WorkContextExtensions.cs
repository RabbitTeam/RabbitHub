using Rabbit.Kernel.Works;
using Rabbit.Web.Works;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Utility.Extensions
{
    /// <summary>
    /// 工作上下文扩展方法。
    /// </summary>
    public static class WorkContextExtensions
    {
        /// <summary>
        /// 获取布局。
        /// </summary>
        /// <param name="workContext">工作上下文。</param>
        /// <returns>布局对象。</returns>
        public static dynamic GetLayout(this WorkContext workContext)
        {
            return workContext.GetState<dynamic>("Layout");
        }

        /// <summary>
        /// 获取工作上下文。
        /// </summary>
        /// <param name="controllerContext">控制器上下文。</param>
        /// <returns>工作上下文。</returns>
        public static WorkContext GetWorkContext(this ControllerContext controllerContext)
        {
            return controllerContext == null ? null : controllerContext.RequestContext.GetWorkContext();
        }
    }
}