using Rabbit.Kernel.Logging;
using System.Linq;
using System.Web.Mvc;
using IFilterProvider = Rabbit.Web.Mvc.Mvc.Filters.IFilterProvider;

namespace Rabbit.Components.Security.Web
{
    /// <summary>
    /// 安全过滤器。
    /// </summary>
    public sealed class SecurityFilter : IFilterProvider, IExceptionFilter, IAuthorizationFilter
    {
        #region Field

        private readonly IAuthorizer _authorizer;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的安全过滤器。
        /// </summary>
        /// <param name="authorizer">授权人。</param>
        public SecurityFilter(IAuthorizer authorizer)
        {
            _authorizer = authorizer;
            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// 日志记录器。
        /// </summary>
        public ILogger Logger { get; set; }

        #endregion Property

        #region Implementation of IAuthorizationFilter

        /// <summary>
        /// 在需要授权时调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var accessFrontEnd = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AlwaysAccessibleAttribute), true).Any();

            if (!Rabbit.Web.Mvc.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext) && !accessFrontEnd && !_authorizer.Authorize(StandardPermissions.AccessFrontEnd))
                filterContext.Result = new HttpUnauthorizedResult();
        }

        #endregion Implementation of IAuthorizationFilter

        #region Implementation of IExceptionFilter

        /// <summary>
        /// 在发生异常时调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnException(ExceptionContext filterContext)
        {
            if (!(filterContext.Exception is RabbitSecurityException))
                return;

            try
            {
                Logger.Information(filterContext.Exception, "安全异常转换为拒绝访问结果。");
            }
            catch
            {
            }

            filterContext.Result = new HttpUnauthorizedResult();
            filterContext.ExceptionHandled = true;
        }

        #endregion Implementation of IExceptionFilter
    }
}