using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.FileSystems.Dependencies
{
    /// <summary>
    /// 依赖项描述符。
    /// </summary>
    public sealed class DependencyDescriptor
    {
        /// <summary>
        /// 初始化一个新的依赖项描述符实例。
        /// </summary>
        public DependencyDescriptor()
        {
            References = Enumerable.Empty<DependencyReferenceDescriptor>();
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 装载机名称。
        /// </summary>
        public string LoaderName { get; set; }

        /// <summary>
        /// 虚拟路径。
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// 引用集合。
        /// </summary>
        public IEnumerable<DependencyReferenceDescriptor> References { get; set; }
    }

    /// <summary>
    /// 依赖项引用描述符。
    /// </summary>
    public sealed class DependencyReferenceDescriptor
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 装载机。
        /// </summary>
        public string LoaderName { get; set; }

        /// <summary>
        /// 虚拟路径。
        /// </summary>
        public string VirtualPath { get; set; }
    }
}