using System.Collections.Generic;

namespace Rabbit.Components.Security.Permissions
{
    /// <summary>
    /// 一个立体的许可类型。
    /// </summary>
    public class PermissionStereotype
    {
        /// <summary>
        /// 许可名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 许可集合。
        /// </summary>
        public IEnumerable<Permission> Permissions { get; set; }
    }
}