using Rabbit.Kernel.Environment.ShellBuilders.Models;

namespace Rabbit.Web.Mvc.Environment.ShellBuilders.Models
{
    internal sealed class ControllerBlueprint : BlueprintItem
    {
        public string AreaName { get; set; }

        public string ControllerName { get; set; }
    }
}