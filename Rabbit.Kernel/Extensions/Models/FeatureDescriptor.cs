using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Extensions.Models
{
    /// <summary>
    /// 特性描述符。
    /// </summary>
    public sealed class FeatureDescriptor
    {
        /// <summary>
        /// 初始化一个特性描述符。
        /// </summary>
        public FeatureDescriptor()
        {
            Dependencies = Enumerable.Empty<string>();
        }

        /// <summary>
        /// 扩展描述符。
        /// </summary>
        public ExtensionDescriptorEntry Extension { get; set; }

        /// <summary>
        /// 标识。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 分类。
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 优先级。
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 依赖特性。
        /// </summary>
        public IEnumerable<string> Dependencies { get; set; }
    }
}