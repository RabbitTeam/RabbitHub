using System.Collections.Generic;

namespace Rabbit.Components.Security.Permissions
{
    /// <summary>
    /// 一个许可模型。
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// 许可名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 许可说明。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 许可分类。
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 隐示通过的许可。
        /// </summary>
        public IEnumerable<Permission> ImpliedBy { get; set; }

        /// <summary>
        /// 通过名称来新建一个许可。
        /// </summary>
        /// <param name="name">许可名称。</param>
        /// <returns>许可模型。</returns>

        public static Permission Named(string name)
        {
            return new Permission { Name = name };
        }
    }
}