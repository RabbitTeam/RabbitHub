using Autofac;
using Autofac.Builder;
using Rabbit.Kernel.Environment.ShellBuilders;
using Rabbit.Kernel.Environment.ShellBuilders.Models;
using Rabbit.Web.Mvc.Environment.Extensions;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc
{
    internal sealed class ShellContainerRegistrations : IShellContainerRegistrations
    {
        #region Implementation of IShellContainerRegistrations

        /// <summary>
        /// 注册动作。
        /// </summary>
        /// <param name="builder">容器建造者。</param>
        /// <param name="blueprint">外壳蓝图。</param>
        public void Registrations(ContainerBuilder builder, ShellBlueprint blueprint)
        {
            foreach (var item in blueprint.GetControllers())
            {
                var serviceKeyName = (item.AreaName + "/" + item.ControllerName).ToLowerInvariant();
                var serviceKeyType = item.Type;
                RegisterType(builder, item)
                    .Keyed<IController>(serviceKeyName)
                    .Keyed<IController>(serviceKeyType)
                    .WithMetadata("ControllerType", item.Type)
                    .InstancePerDependency();
            }

            foreach (var item in blueprint.GetHttpControllers())
            {
                var serviceKeyName = (item.AreaName + "/" + item.ControllerName).ToLowerInvariant();
                var serviceKeyType = item.Type;
                RegisterType(builder, item)
                    .Keyed<IHttpController>(serviceKeyName)
                    .Keyed<IHttpController>(serviceKeyType)
                    .WithMetadata("ControllerType", item.Type)
                    .InstancePerDependency();
            }
        }

        #endregion Implementation of IShellContainerRegistrations

        #region Private Method

        private static IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(ContainerBuilder builder, BlueprintItem item)
        {
            return builder.RegisterType(item.Type)
                .WithProperty("Feature", item.Feature)
                .WithMetadata("Feature", item.Feature);
        }

        #endregion Private Method
    }
}