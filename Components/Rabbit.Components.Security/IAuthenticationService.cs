using Rabbit.Kernel;

namespace Rabbit.Components.Security
{
    /// <summary>
    /// 一个抽象的认证服务。
    /// </summary>
    public interface IAuthenticationService : IDependency
    {
        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="user">用户模型。</param>
        /// <param name="createPersistentCookie">是否创建持久的Cookie。</param>
        void SignIn(IUser user, bool createPersistentCookie);

        /// <summary>
        /// 登出。
        /// </summary>
        void SignOut();

        /// <summary>
        /// 为当前请求设置一个身份认证。
        /// </summary>
        /// <param name="user">用户模型。</param>
        void SetAuthenticatedUserForRequest(IUser user);

        /// <summary>
        /// 获取当前认证的用户。
        /// </summary>
        /// <returns>用户模型。</returns>
        IUser GetAuthenticatedUser();
    }
}