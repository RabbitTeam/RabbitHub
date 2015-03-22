using Rabbit.Kernel.Environment.ShellBuilders.Models;

namespace Rabbit.Components.Data.Models
{
    /// <summary>
    /// 记录蓝图项。
    /// </summary>
    public sealed class RecordBlueprint : BlueprintItem
    {
        /// <summary>
        /// 表名称。
        /// </summary>
        public string TableName { get; set; }
    }
}