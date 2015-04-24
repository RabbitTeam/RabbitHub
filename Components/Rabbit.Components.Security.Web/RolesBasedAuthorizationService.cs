using Rabbit.Components.Security.Permissions;
using Rabbit.Kernel.Localization;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility.Extensions;
using Rabbit.Kernel.Works;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Components.Security.Web
{
    internal sealed class RolesBasedAuthorizationService : IAuthorizationService
    {
        #region Field

        private readonly IRoleService _roleService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IEnumerable<IAuthorizationServiceEventHandler> _authorizationServiceEventHandlers;

        /// <summary>
        /// 匿名用户角色。
        /// </summary>
        private static readonly string[] AnonymousRole = { "Anonymous" };

        /// <summary>
        /// 已授权用户角色。
        /// </summary>
        private static readonly string[] AuthenticatedRole = { "Authenticated" };

        #endregion Field

        #region Constructor

        public RolesBasedAuthorizationService(IRoleService roleService, IWorkContextAccessor workContextAccessor, IEnumerable<IAuthorizationServiceEventHandler> authorizationServiceEventHandlers)
        {
            _roleService = roleService;
            _workContextAccessor = workContextAccessor;
            _authorizationServiceEventHandlers = authorizationServiceEventHandlers;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Property

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        #endregion Property

        #region Implementation of IAuthorizationService

        /// <summary>
        /// 检查访问权限。
        /// </summary>
        /// <param name="permission">需要的许可。</param>
        /// <param name="user">用户模型。</param>
        /// <exception cref="ArgumentNullException"><paramref name="permission"/> 为 null。</exception>
        /// <exception cref="RabbitSecurityException">检查权限失败。</exception>
        public void CheckAccess(Permission permission, IUser user)
        {
            permission.NotNull("permission");
            if (!TryCheckAccess(permission, user))
            {
                throw new RabbitSecurityException(T("发生一个安全异常。"))
                {
                    PermissionName = permission.Name,
                    User = user
                };
            }
        }

        /// <summary>
        /// 尝试检查访问权限。
        /// </summary>
        /// <param name="permission">需要的许可。</param>
        /// <param name="user">用户模型。</param>
        /// <returns>如果可以访问则返回true，否则返回false。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="permission"/> 为 null。</exception>
        public bool TryCheckAccess(Permission permission, IUser user)
        {
            permission.NotNull("permission");

            var context = new CheckAccessContext { Permission = permission, User = user };
            foreach (var authorizationServiceEventHandler in _authorizationServiceEventHandlers)
                authorizationServiceEventHandler.Checking(context);

            //尝试3次授权检查。
            for (var adjustmentLimiter = 0; adjustmentLimiter != 3; ++adjustmentLimiter)
            {
                //未通过认证并且当前用户不等于null。
                if (!context.Granted && context.User != null)
                {
                    //如果当前用户是超级用户则通过检查。
                    if (string.Equals(context.User.UserName, _workContextAccessor.GetContext().CurrentTenant.SuperUser, StringComparison.Ordinal))
                        context.Granted = true;
                }

                //未通过认证。
                if (!context.Granted)
                {
                    //确定哪组权限将满足访问检查（当前许可、所有者许可、隐含许可）
                    var grantingNames = PermissionNames(context.Permission, Enumerable.Empty<string>()).Distinct().ToArray();

                    //确定哪些组角色应该由访问检查检查
                    IEnumerable<string> rolesToExamine;

                    //如果没有用户则标记为匿名角色组。
                    if (context.User == null)
                        rolesToExamine = AnonymousRole;
                    else if (context.User is IUserRoles)
                    {
                        //当前用户不为空，所以得到他的角色，并添加“已验证”
                        rolesToExamine = (context.User as IUserRoles).Roles;

                        //当它是在管理模拟匿名用户
                        if (!rolesToExamine.Contains(AnonymousRole[0]))
                        {
                            rolesToExamine = rolesToExamine.Concat(AuthenticatedRole);
                        }
                    }
                    else
                    {
                        //用户不为空，也没有特定的角色，那么它只是“身份验证”
                        rolesToExamine = AuthenticatedRole;
                    }

                    if (rolesToExamine != null)
                    {
                        foreach (var role in rolesToExamine.ToArray())
                        {
                            foreach (var permissionName in _roleService.GetPermissionsForRoleByName(role))
                            {
                                var possessedName = permissionName;
                                if (grantingNames.Any(grantingName => string.Equals(possessedName, grantingName, StringComparison.OrdinalIgnoreCase)))
                                {
                                    context.Granted = true;
                                }

                                if (context.Granted)
                                    break;
                            }

                            if (context.Granted)
                                break;
                        }
                    }
                }

                context.Adjusted = false;
                //进行调整。
                foreach (var authorizationServiceEventHandler in _authorizationServiceEventHandlers)
                    authorizationServiceEventHandler.Adjust(context);
                //如果不再需要调整则退出许可检查。
                if (!context.Adjusted)
                    break;
            }

            //完成授权。
            foreach (var authorizationServiceEventHandler in _authorizationServiceEventHandlers)
                authorizationServiceEventHandler.Complete(context);

            //是否通过授权检查。
            return context.Granted;
        }

        #endregion Implementation of IAuthorizationService

        #region Private Method

        private static IEnumerable<string> PermissionNames(Permission permission, IEnumerable<string> stack)
        {
            //给予当前许可名称。
            yield return permission.Name;

            //迭代隐含的许可。
            if (permission.ImpliedBy != null && permission.ImpliedBy.Any())
            {
                stack = stack.ToArray();
                foreach (var impliedBy in permission.ImpliedBy)
                {
                    //避免潜在的递归。
                    if (stack.Contains(impliedBy.Name))
                        continue;

                    //递归隐含的许可名称。
                    foreach (var impliedName in PermissionNames(impliedBy, stack.Concat(new[] { permission.Name })))
                    {
                        yield return impliedName;
                    }
                }
            }

            //所有者拥有所有许可。
            yield return StandardPermissions.Owner.Name;
        }

        #endregion Private Method
    }
}