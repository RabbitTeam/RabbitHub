using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Environment.Descriptor.Models
{
    /// <summary>
    /// 外壳描述符。
    /// </summary>
    public sealed class ShellDescriptor
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的外壳描述符。
        /// </summary>
        public ShellDescriptor()
        {
            Features = Enumerable.Empty<ShellFeature>();
        }

        #endregion Constructor

        /// <summary>
        /// 序列号。
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// 特性集合。
        /// </summary>
        public IEnumerable<ShellFeature> Features { get; set; }
    }

    /// <summary>
    /// 外壳特性。
    /// </summary>
    public sealed class ShellFeature
    {
        /// <summary>
        /// 特性名称。
        /// </summary>
        public string Name { get; set; }
    }
}