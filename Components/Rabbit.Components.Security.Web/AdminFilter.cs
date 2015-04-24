using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Localization;
using Rabbit.Web.Mvc.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using IFilterProvider = Rabbit.Web.Mvc.Mvc.Filters.IFilterProvider;

namespace Rabbit.Components.Security.Web
{
    /// <summary>
    /// 管理员过滤器。
    /// </summary>
    [SuppressDependency("Rabbit.Web.Mvc.UI.Admin.AdminFilter")]
    internal sealed class AdminFilter : IFilterProvider, IAuthorizationFilter
    {
        #region Field

        private readonly IAuthorizer _authorizer;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的管理过滤器。
        /// </summary>
        /// <param name="authorizer">授权人。</param>
        public AdminFilter(IAuthorizer authorizer)
        {
            _authorizer = authorizer;
            T = NullLocalizer.Instance;
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// 获取本地化字符串委托。
        /// </summary>
        public Localizer T { get; set; }

        #endregion Property

        #region Implementation of IAuthorizationFilter

        /// <summary>
        /// 在需要授权时调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!IsAdmin(filterContext))
                return;
            if (!_authorizer.Authorize(StandardPermissions.AccessAdminPanel, T("不能访问管理页面")))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }

            Apply(filterContext.RequestContext);
        }

        #endregion Implementation of IAuthorizationFilter

        #region Private Method

        private static void Apply(RequestContext context)
        {
            if (context != null)
                context.RouteData.DataTokens[typeof(Rabbit.Web.Mvc.UI.Admin.AdminFilter).FullName] = null;
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