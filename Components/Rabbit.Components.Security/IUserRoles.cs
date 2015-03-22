using System.Collections.Generic;

namespace Rabbit.Components.Security
{
    /// <summary>
    /// 一个抽象的用户角色接口。
    /// </summary>
    public interface IUserRoles
    {
        /// <summary>
        /// 用户所有角色。
        /// </summary>
        IList<string> Roles { get; }
    }
}