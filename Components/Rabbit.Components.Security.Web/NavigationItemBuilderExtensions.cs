using Rabbit.Components.Security.Permissions;
using Rabbit.Kernel.Utility.Extensions;
using Rabbit.Web.UI.Navigation;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Components.Security.Web
{
    /// <summary>
    /// 导航项建设者扩展方法。
    /// </summary>
    public static class NavigationItemBuilderExtensions
    {
        /// <summary>
        /// 许可。
        /// </summary>
        /// <param name="builder">导航项建设者。</param>
        /// <param name="permissions">许可模型。</param>
        /// <returns>导航项建设者。</returns>
        public static NavigationItemBuilder Permission(this NavigationItemBuilder builder, params Permission[] permissions)
        {
            permissions.NotNull("permissions");

            const string name = "Permissions";
            var permissionList = builder.MenuItem.GetAttribute<IEnumerable<Permission>>(name) ?? Enumerable.Empty<Permission>();
            builder.MenuItem.SetAttribute(name, permissionList.Concat(permissions));

            return builder;
        }
    }
}