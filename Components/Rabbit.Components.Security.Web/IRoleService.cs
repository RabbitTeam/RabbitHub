using Rabbit.Kernel;
using System.Collections.Generic;

namespace Rabbit.Components.Security.Web
{
    /// <summary>
    /// 一个抽象的角色服务。
    /// </summary>
    public interface IRoleService : IDependency
    {
        /// <summary>
        /// 根级角色的名称获取该角色所拥有的权限。
        /// </summary>
        /// <param name="name">角色名称。</param>
        /// <returns>该角色所拥有的权限。</returns>
        IEnumerable<string> GetPermissionsForRoleByName(string name);
    }
}