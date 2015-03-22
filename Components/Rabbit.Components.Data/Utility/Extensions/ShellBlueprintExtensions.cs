using Rabbit.Components.Data.Models;
using Rabbit.Kernel.Environment.ShellBuilders.Models;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Components.Data.Utility.Extensions
{
    /// <summary>
    /// 外壳蓝图扩展。
    /// </summary>
    public static class ShellBlueprintExtensions
    {
        /// <summary>
        /// 获取蓝图里面的记录蓝图。
        /// </summary>
        /// <param name="shellBlueprint">外壳蓝图。</param>
        /// <returns>记录蓝图。</returns>
        public static IEnumerable<RecordBlueprint> GetRecords(this ShellBlueprint shellBlueprint)
        {
            return shellBlueprint["Records"] as IEnumerable<RecordBlueprint> ?? Enumerable.Empty<RecordBlueprint>();
        }

        /// <summary>
        /// 设置外壳蓝图里的记录蓝图。
        /// </summary>
        /// <param name="shellBlueprint">外壳蓝图。</param>
        /// <param name="records">记录蓝图。</param>
        public static void SetRecords(this ShellBlueprint shellBlueprint, IEnumerable<RecordBlueprint> records)
        {
            shellBlueprint["Records"] = records;
        }
    }
}