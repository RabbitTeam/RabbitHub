using Rabbit.Components.Security.Permissions;
using Rabbit.Kernel.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Components.Security.Web
{
    /// <summary>
    /// 标准权限。
    /// </summary>
    public sealed class StandardPermissions : IPermissionProvider
    {
        #region Field

        /// <summary>
        /// 管理面板权限。
        /// </summary>
        public static readonly Permission AccessAdminPanel = new Permission { Name = "AccessAdminPanel", Description = "管理面板访问" };

        /// <summary>
        /// 前端访问权限。
        /// </summary>
        public static readonly Permission AccessFrontEnd = new Permission { Name = "AccessFrontEnd", Description = "前端访问" };

        /// <summary>
        /// 所有者。
        /// </summary>
        public static readonly Permission Owner = new Permission { Name = "Owner", Description = "租户所有者权限" };

        #endregion Field

        #region Implementation of IPermissionProvider

        /// <summary>
        /// 功能。
        /// </summary>
        public Feature Feature
        {
            get
            {
                return new Feature
                {
                    Descriptor = new FeatureDescriptor
                    {
                        Id = "Rabbit.Web",
                        Category = "Core",
                        Dependencies = Enumerable.Empty<string>(),
                        Description = string.Empty,
                        Extension = new ExtensionDescriptorEntry(new ExtensionDescriptor(), "Rabbit.Web", null, null)
                    },
                    ExportedTypes = Enumerable.Empty<Type>()
                };
            }
        }

        /// <summary>
        /// 获取许可集合。
        /// </summary>
        /// <returns>许可集合。</returns>
        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                AccessAdminPanel,
                AccessFrontEnd,
                Owner
            };
        }

        /// <summary>
        /// 获取默认的立体许可集合。
        /// </summary>
        /// <returns>立体许可集合。</returns>
        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                //管理员
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {Owner, AccessAdminPanel}
                },
                //匿名用户
                new PermissionStereotype {
                    Name = "Anonymous",
                    Permissions = new[] {AccessFrontEnd}
                },
                //授权后的用户
                new PermissionStereotype
                {
                    Name = "Authenticated",
                    Permissions = new[] {AccessFrontEnd}
                }
            };
        }

        #endregion Implementation of IPermissionProvider
    }
}