using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Services;
using Rabbit.Web;
using System;
using System.Web;
using System.Web.Security;

namespace Rabbit.Components.Security.Web
{
    /// <summary>
    /// 基于表单授权的服务基类。
    /// </summary>
    public abstract class FormsAuthenticationServiceBase : IAuthenticationService
    {
        #region Field

        private readonly ShellSettings _settings;
        private readonly IClock _clock;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IUser _signedInUser;
        private bool _isAuthenticated;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个表单授权服务。
        /// </summary>
        /// <param name="settings">租户设置。</param>
        /// <param name="clock">时钟服务。</param>
        /// <param name="httpContextAccessor">HttpContext访问器。</param>
        protected FormsAuthenticationServiceBase(ShellSettings settings, IClock clock, IHttpContextAccessor httpContextAccessor)
        {
            _settings = settings;
            _clock = clock;
            _httpContextAccessor = httpContextAccessor;

            Logger = NullLogger.Instance;
            ExpirationTimeSpan = TimeSpan.FromDays(30);
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// 日志记录器。
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 过期时间区。
        /// </summary>
        public TimeSpan ExpirationTimeSpan { get; set; }

        #endregion Property

        #region Implementation of IAuthenticationService

        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="user">用户模型。</param>
        /// <param name="createPersistentCookie">是否创建持久的Cookie。</param>
        public virtual void SignIn(IUser user, bool createPersistentCookie)
        {
            var now = _clock.UtcNow.ToLocalTime();

            var userData = string.Concat(Convert.ToString(user.Identity), ";", _settings.Name);

            var ticket = new FormsAuthenticationTicket(
                1,
                user.UserName,
                now,
                now.Add(ExpirationTimeSpan),
                createPersistentCookie,
                userData,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };

            var httpContext = _httpContextAccessor.Current();

            if (!string.IsNullOrEmpty(_settings["RequestUrlPrefix"]))
            {
                cookie.Path = GetCookiePath(httpContext);
            }

            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            if (createPersistentCookie)
            {
                cookie.Expires = ticket.Expiration;
            }

            httpContext.Response.Cookies.Add(cookie);

            _isAuthenticated = true;
            _signedInUser = user;
        }

        /// <summary>
        /// 登出。
        /// </summary>
        public virtual void SignOut()
        {
            _signedInUser = null;
            _isAuthenticated = false;
            FormsAuthentication.SignOut();

            var httpContext = _httpContextAccessor.Current();
            var formsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty)
            {
                Expires = DateTime.Now.AddYears(-1),
            };

            if (!string.IsNullOrEmpty(_settings["RequestUrlPrefix"]))
            {
                formsCookie.Path = GetCookiePath(httpContext);
            }

            httpContext.Response.Cookies.Add(formsCookie);
        }

        /// <summary>
        /// 为当前请求设置一个身份认证。
        /// </summary>
        /// <param name="user">用户模型。</param>
        public void SetAuthenticatedUserForRequest(IUser user)
        {
            _signedInUser = user;
            _isAuthenticated = true;
        }

        /// <summary>
        /// 获取当前认证的用户。
        /// </summary>
        /// <returns>用户模型。</returns>
        public IUser GetAuthenticatedUser()
        {
            if (_signedInUser != null || _isAuthenticated)
                return _signedInUser;

            var httpContext = _httpContextAccessor.Current();
            if (httpContext == null || !httpContext.Request.IsAuthenticated || !(httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)httpContext.User.Identity;
            var userData = formsIdentity.Ticket.UserData ?? string.Empty;

            var userDataSegments = userData.Split(';');

            if (userDataSegments.Length < 2)
                return null;

            var userDataIdentity = userDataSegments[0];
            var userDataTenant = userDataSegments[1];

            if (!string.Equals(userDataTenant, _settings.Name, StringComparison.Ordinal))
                return null;

            var user = GetUserByIdentity(userDataIdentity);
            if (user == null)
            {
                Logger.Fatal(string.Format("根据用户标识 {0} 没有找到用户信息。", userDataIdentity));
                return null;
            }

            _isAuthenticated = true;
            return _signedInUser = user;
        }

        #endregion Implementation of IAuthenticationService

        #region Abstract Method

        /// <summary>
        /// 根据用户标识获取用户信息。
        /// </summary>
        /// <param name="identity">用户标识。</param>
        /// <returns>用户模型。</returns>
        protected abstract IUser GetUserByIdentity(string identity);

        #endregion Abstract Method

        #region Private Method

        private string GetCookiePath(HttpContextBase httpContext)
        {
            var cookiePath = httpContext.Request.ApplicationPath;
            if (cookiePath != null && cookiePath.Length > 1)
            {
                cookiePath += '/';
            }

            cookiePath += _settings["RequestUrlPrefix"];

            return cookiePath;
        }

        #endregion Private Method
    }
}