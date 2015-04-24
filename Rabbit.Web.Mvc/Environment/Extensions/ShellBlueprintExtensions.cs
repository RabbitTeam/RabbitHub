using Rabbit.Kernel.Environment.ShellBuilders.Models;
using Rabbit.Web.Mvc.Environment.ShellBuilders.Models;
using System.Collections.Generic;

namespace Rabbit.Web.Mvc.Environment.Extensions
{
    /// <summary>
    /// 外壳蓝图扩展方法。
    /// </summary>
    internal static class ShellBlueprintExtensions
    {
        public static IEnumerable<ControllerBlueprint> GetControllers(this ShellBlueprint blueprint)
        {
            return blueprint["Controllers"] as IEnumerable<ControllerBlueprint>;
        }

        public static IEnumerable<ControllerBlueprint> GetHttpControllers(this ShellBlueprint blueprint)
        {
            return blueprint["HttpControllers"] as IEnumerable<ControllerBlueprint>;
        }

        public static void SetControllers(this ShellBlueprint blueprint, IEnumerable<ControllerBlueprint> blueprints)
        {
            blueprint["Controllers"] = blueprints;
        }

        public static void SetHttpControllers(this ShellBlueprint blueprint, IEnumerable<ControllerBlueprint> blueprints)
        {
            blueprint["HttpControllers"] = blueprints;
        }
    }
}