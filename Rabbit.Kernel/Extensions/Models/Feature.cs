using System;
using System.Collections.Generic;

namespace Rabbit.Kernel.Extensions.Models
{
    /// <summary>
    /// 特性条目。
    /// </summary>
    public sealed class Feature
    {
        /// <summary>
        /// 特性描述符。
        /// </summary>
        public FeatureDescriptor Descriptor { get; set; }

        /// <summary>
        /// 特性中可导出的类型。
        /// </summary>
        public IEnumerable<Type> ExportedTypes { get; set; }
    }
}