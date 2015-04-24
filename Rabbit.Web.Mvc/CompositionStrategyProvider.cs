using Rabbit.Kernel.Environment.ShellBuilders;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Web.Mvc.Environment.Extensions;
using Rabbit.Web.Mvc.Environment.ShellBuilders.Models;
using System;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc
{
    internal sealed class CompositionStrategyProvider : ICompositionStrategyProvider
    {
        #region Implementation of ICompositionStrategyProvider

        /// <summary>
        /// 应用。
        /// </summary>
        /// <param name="context">组合策略应用上下文。</param>
        public void Apply(CompositionStrategyApplyContext context)
        {
            var controllers = context.BuildBlueprint(IsController, BuildController);
            var httpControllers = context.BuildBlueprint(IsHttpController, BuildController);

            context.ShellBlueprint.SetControllers(controllers);
            context.ShellBlueprint.SetHttpControllers(httpControllers);
        }

        #endregion Implementation of ICompositionStrategyProvider

        #region Private Method

        private static ControllerBlueprint BuildController(Type type, Feature feature)
        {
            var areaName = feature.Descriptor.Extension.Id;

            var controllerName = type.Name;
            if (controllerName.EndsWith("Controller"))
                controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);

            return new ControllerBlueprint
            {
                Type = type,
                Feature = feature,
                AreaName = areaName,
                ControllerName = controllerName,
            };
        }

        private static bool IsController(Type type)
        {
            return typeof(IController).IsAssignableFrom(type);
        }

        private static bool IsHttpController(Type type)
        {
            return typeof(IHttpController).IsAssignableFrom(type);
        }

        #endregion Private Method
    }
}