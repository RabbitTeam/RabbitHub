using Rabbit.Kernel;
using Rabbit.Kernel.Extensions.Models;
using System.Collections.Generic;

namespace Rabbit.Components.Security.Permissions
{
    /// <summary>
    /// 一个抽象的许可提供程序。
    /// </summary>
    public interface IPermissionProvider : IDependency
    {
        /// <summary>
        /// 功能。
        /// </summary>
        Feature Feature { get; }

        /// <summary>
        /// 获取许可集合。
        /// </summary>
        /// <returns>许可集合。</returns>
        IEnumerable<Permission> GetPermissions();

        /// <summary>
        /// 获取默认的立体许可集合。
        /// </summary>
        /// <returns>立体许可集合。</returns>
        IEnumerable<PermissionStereotype> GetDefaultStereotypes();
    }
}