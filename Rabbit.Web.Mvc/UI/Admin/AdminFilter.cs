using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.UI.Admin
{
    /// <summary>
    /// 管理员过滤器。
    /// </summary>
    public sealed class AdminFilter : Mvc.Filters.IFilterProvider, IAuthorizationFilter
    {
        #region Implementation of IAuthorizationFilter

        /// <summary>
        /// 在需要授权时调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (IsAdmin(filterContext))
            {
                /*                if (!_authorizer.Authorize(StandardPermissions.AccessAdminPanel, T("不能访问管理页面")))
                                {
                                    filterContext.Result = new HttpUnauthorizedResult();
                                }*/

                Apply(filterContext.RequestContext);
            }
        }

        #endregion Implementation of IAuthorizationFilter

        /// <summary>
        /// 是否应用了管理过滤器。
        /// </summary>
        /// <param name="context">请求上下文。</param>
        /// <returns>如果应用了管理过滤器则返回true，否则返回false。</returns>
        public static bool IsApplied(RequestContext context)
        {
            return context != null && context.RouteData.DataTokens.ContainsKey(typeof(AdminFilter).FullName);
            //            return context.HttpContext.Items.Contains(typeof(AdminFilter));
        }

        #region Private Method

        private static void Apply(RequestContext context)
        {
            if (context != null)
                context.RouteData.DataTokens[typeof(AdminFilter).FullName] = null;
            //            context.HttpContext.Items[typeof(AdminFilter)] = null;
        }

        private static bool IsAdmin(AuthorizationContext filterContext)
        {
            if (IsNameAdmin(filterContext) || IsNameAdminProxy(filterContext))
            {
                return true;
            }

            var adminAttributes = GetAdminAttributes(filterContext.ActionDescriptor);
            return adminAttributes != null && adminAttributes.Any();
        }

        private static bool IsNameAdmin(AuthorizationContext filterContext)
        {
            return string.Equals(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "Admin",
                                 StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsNameAdminProxy(AuthorizationContext filterContext)
        {
            return filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.StartsWith(
                "AdminControllerProxy", StringComparison.InvariantCultureIgnoreCase) &&
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Length == "AdminControllerProxy".Length + 32;
        }

        private static IEnumerable<AdminAttribute> GetAdminAttributes(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(AdminAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(AdminAttribute), true))
                .OfType<AdminAttribute>();
        }

        #endregion Private Method
    }
}