using System;

namespace Rabbit.Components.Security
{
    internal class NullAuthenticationService : IAuthenticationService
    {
        #region Implementation of IAuthenticationService

        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="user">用户模型。</param>
        /// <param name="createPersistentCookie">是否创建持久的Cookie。</param>
        public void SignIn(IUser user, bool createPersistentCookie)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 登出。
        /// </summary>
        public void SignOut()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 为当前请求设置一个身份认证。
        /// </summary>
        /// <param name="user">用户模型。</param>
        public void SetAuthenticatedUserForRequest(IUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取当前认证的用户。
        /// </summary>
        /// <returns>用户模型。</returns>
        public IUser GetAuthenticatedUser()
        {
            throw new NotImplementedException();
        }

        #endregion Implementation of IAuthenticationService
    }
}