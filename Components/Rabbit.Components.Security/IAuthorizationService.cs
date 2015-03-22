using Rabbit.Components.Security.Permissions;
using Rabbit.Kernel;
using System;

namespace Rabbit.Components.Security
{
    /// <summary>
    /// 一个抽象的授权服务。
    /// </summary>
    public interface IAuthorizationService : IDependency
    {
        /// <summary>
        /// 检查访问权限。
        /// </summary>
        /// <param name="permission">需要的许可。</param>
        /// <param name="user">用户模型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="permission"/> 为 null。</exception>
        /// <exception cref="RabbitSecurityException">检查权限失败。</exception>
        void CheckAccess(Permission permission, IUser user);

        /// <summary>
        /// 尝试检查访问权限。
        /// </summary>
        /// <param name="permission">需要的许可。</param>
        /// <param name="user">用户模型。</param>
        /// <returns>如果可以访问则返回true，否则返回false。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="permission"/> 为 null。</exception>
        bool TryCheckAccess(Permission permission, IUser user);
    }
}