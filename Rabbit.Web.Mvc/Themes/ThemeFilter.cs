using Rabbit.Web.Mvc.UI.Admin;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.Themes
{
    /// <summary>
    /// 主题过滤器。
    /// </summary>
    internal sealed class ThemeFilter : Mvc.Filters.IFilterProvider, IActionFilter, IResultFilter
    {
        #region Implementation of IActionFilter

        /// <summary>
        /// 在执行操作方法之前调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var attribute = GetThemedAttribute(filterContext.ActionDescriptor);
            if (AdminFilter.IsApplied(filterContext.RequestContext))
            {
                //如果是管理员则是默认主题
                if (attribute == null || attribute.Enabled)
                {
                    Apply(filterContext.RequestContext);
                }
            }
            else
            {
                //非管理员显式主题
                //如果是一个管理员控制器或主题被禁用则不使用布局结果
                if (attribute != null && attribute.Enabled)
                {
                    Apply(filterContext.RequestContext);
                }
            }
        }

        /// <summary>
        /// 在执行操作方法后调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        #endregion Implementation of IActionFilter

        #region Implementation of IResultFilter

        /// <summary>
        /// 在操作结果执行之前调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }

        /// <summary>
        /// 在操作结果执行后调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        #endregion Implementation of IResultFilter

        internal static bool IsApplied(RequestContext context)
        {
            return context != null && context.RouteData.DataTokens.ContainsKey(typeof(ThemeFilter).FullName);
            //            return context.HttpContext.Items.Contains(typeof(ThemeFilter));
        }

        #region Private Method

        private static ThemedAttribute GetThemedAttribute(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(ThemedAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(ThemedAttribute), true))
                .OfType<ThemedAttribute>()
                .FirstOrDefault();
        }

        private static void Apply(RequestContext context)
        {
            context.RouteData.DataTokens[typeof(ThemeFilter).FullName] = null;
            //            context.HttpContext.Items[typeof(ThemeFilter)] = null;
        }

        #endregion Private Method
    }
}