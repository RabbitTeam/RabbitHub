using Rabbit.Kernel.Extensions.Models;
using System;

namespace Rabbit.Kernel.Environment.ShellBuilders.Models
{
    /// <summary>
    /// 蓝图项。
    /// </summary>
    public class BlueprintItem
    {
        /// <summary>
        /// 类型。
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 特性条目。
        /// </summary>
        public Feature Feature { get; set; }
    }
}