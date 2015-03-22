using Rabbit.Components.Security.Permissions;
using Rabbit.Kernel;
using Rabbit.Kernel.Localization;
using Rabbit.Kernel.Works;

namespace Rabbit.Components.Security
{
    /// <summary>
    /// 一个抽象的授权人。
    /// </summary>
    public interface IAuthorizer : IDependency
    {
        /// <summary>
        /// 针对一个全新对当前用户授权。
        /// </summary>
        /// <param name="permission">针对的授权许可。</param>
        /// <returns>如果授权成功则返回true，否则返回false。</returns>
        bool Authorize(Permission permission);

        /// <summary>
        /// 对授权许可的当前用户;如果授权失败，将显示指定的消息。
        /// </summary>
        /// <param name="permission">针对的授权许可。</param>
        /// <param name="message">要显示本地化的消息，如果授权失败。</param>
        /// <returns>如果授权成功则返回true，否则返回false。</returns>
        bool Authorize(Permission permission, LocalizedString message);
    }

    internal sealed class Authorizer : IAuthorizer
    {
        #region Field

        private readonly IAuthorizationService _authorizationService;
        private readonly IWorkContextAccessor _workContextAccessor;

        #endregion Field

        #region Constructor

        public Authorizer(IAuthorizationService authorizationService, IWorkContextAccessor workContextAccessor)
        {
            _authorizationService = authorizationService;
            _workContextAccessor = workContextAccessor;

            T = NullLocalizer.Instance;
        }

        #endregion Constructor

        #region Property

        public Localizer T { get; set; }

        #endregion Property

        #region Implementation of IAuthorizer

        /// <summary>
        /// 针对一个全新对当前用户授权。
        /// </summary>
        /// <param name="permission">针对的授权许可。</param>
        /// <returns>如果授权成功则返回true，否则返回false。</returns>
        public bool Authorize(Permission permission)
        {
            var currentUser = _workContextAccessor.GetContext().GetState<IUser>("CurrentUser");

            return _authorizationService.TryCheckAccess(permission, currentUser);
        }

        /// <summary>
        /// 对授权许可的当前用户;如果授权失败，将显示指定的消息。
        /// </summary>
        /// <param name="permission">针对的授权许可。</param>
        /// <param name="message">要显示本地化的消息，如果授权失败。</param>
        /// <returns>如果授权成功则返回true，否则返回false。</returns>
        public bool Authorize(Permission permission, LocalizedString message)
        {
            return Authorize(permission);
        }

        #endregion Implementation of IAuthorizer
    }
}