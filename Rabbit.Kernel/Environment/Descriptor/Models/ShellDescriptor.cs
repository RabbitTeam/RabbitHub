using System;
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

    /// <summary>
    /// 外壳特性相等比较器。
    /// </summary>
    public sealed class ShellFeatureEqualityComparer : IEqualityComparer<ShellFeature>
    {
        #region Implementation of IEqualityComparer<in ShellFeature>

        /// <summary>
        /// 确定指定的对象是否相等。
        /// </summary>
        /// <returns>
        /// 如果指定的对象相等，则为 true；否则为 false。
        /// </returns>
        /// <param name="x">要比较的第一个类型为 <see cref="ShellFeature"/> 的对象。</param><param name="y">要比较的第二个类型为 <see cref="ShellFeature"/> 的对象。</param>
        public bool Equals(ShellFeature x, ShellFeature y)
        {
            return x == y || string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 返回指定对象的哈希代码。
        /// </summary>
        /// <returns>
        /// 指定对象的哈希代码。
        /// </returns>
        /// <param name="obj"><see cref="T:System.Object"/>，将为其返回哈希代码。</param><exception cref="T:System.ArgumentNullException"><paramref name="obj"/> 的类型为引用类型，<paramref name="obj"/> 为 null。</exception>
        public int GetHashCode(ShellFeature obj)
        {
            return obj == null ? 0 : obj.Name.ToLower().GetHashCode();
        }

        #endregion Implementation of IEqualityComparer<in ShellFeature>
    }
}