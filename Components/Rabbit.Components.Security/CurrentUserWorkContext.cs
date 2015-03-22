using Rabbit.Kernel.Works;
using System;

namespace Rabbit.Components.Security
{
    internal sealed class CurrentUserWorkContext : IWorkContextStateProvider
    {
        #region Field

        private readonly IAuthenticationService _authenticationService;

        #endregion Field

        #region Constructor

        public CurrentUserWorkContext(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        #endregion Constructor

        #region Implementation of IWorkContextStateProvider

        /// <summary>
        /// 获取状态信值。
        /// </summary>
        /// <typeparam name="T">状态类型。</typeparam>
        /// <param name="name">状态名称。</param>
        /// <returns>获取状态值的委托。</returns>
        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name == "CurrentUser")
                return ctx => (T)_authenticationService.GetAuthenticatedUser();
            return null;
        }

        #endregion Implementation of IWorkContextStateProvider
    }
}