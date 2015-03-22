using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.Descriptor.Models;
using System;
using System.Collections.Generic;

namespace Rabbit.Kernel.Environment.ShellBuilders.Models
{
    /// <summary>
    /// 外壳蓝图。
    /// </summary>
    public sealed class ShellBlueprint
    {
        #region Field

        private readonly IDictionary<string, object> _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        #endregion Field

        /// <summary>
        /// 外壳设置。
        /// </summary>
        public ShellSettings Settings { get; set; }

        /// <summary>
        /// 外壳描述符。
        /// </summary>
        public ShellDescriptor Descriptor { get; set; }

        /// <summary>
        /// 依赖项集合。
        /// </summary>
        public IEnumerable<DependencyBlueprintItem> Dependencies { get; set; }

        /// <summary>
        /// 外壳蓝图索引器。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>值。</returns>
        public object this[string key]
        {
            get
            {
                object retVal;
                return _values.TryGetValue(key, out retVal) ? retVal : null;
            }
            set { _values[key] = value; }
        }

        /// <summary>
        /// 所有外壳蓝图键。
        /// </summary>
        public IEnumerable<string> Keys { get { return _values.Keys; } }
    }
}