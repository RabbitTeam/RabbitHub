using System;
using System.Collections.Generic;

namespace Rabbit.Kernel.Extensions.Models
{
    /// <summary>
    /// 扩展描述符。
    /// </summary>
    public sealed class ExtensionDescriptor
    {
        #region Field

        private readonly IDictionary<string, string> _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        #endregion Field

        #region Property

        /// <summary>
        /// 路径。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 扩展版本。
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// 特性集合。
        /// </summary>
        public IEnumerable<FeatureDescriptor> Features { get; set; }

        /// <summary>
        /// 依赖特性。
        /// </summary>
        public IEnumerable<string> Dependencies { get; set; }

        /// <summary>
        /// 内核版本范围。
        /// </summary>
        public VersionRange KernelVersion { get; set; }

        /// <summary>
        /// 扩展描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 扩展标签。
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 作者。
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 公司。
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 扩展网站。
        /// </summary>
        public string WebSite { get; set; }

        #endregion Property

        /// <summary>
        /// 扩展描述符设置索引器。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>值。</returns>
        public string this[string key]
        {
            get
            {
                string retVal;
                return _values.TryGetValue(key, out retVal) ? retVal : null;
            }
            set { _values[key] = value; }
        }

        /// <summary>
        /// 所有扩展描述符设置键。
        /// </summary>
        public IEnumerable<string> Keys { get { return _values.Keys; } }
    }

    /// <summary>
    /// 扩展描述符条目。
    /// </summary>
    public sealed class ExtensionDescriptorEntry
    {
        /// <summary>
        /// 初始化一个新的扩展描述符条目。
        /// </summary>
        /// <param name="descriptor">扩展描述符。</param>
        /// <param name="id">扩展Id。</param>
        /// <param name="extensionType">扩展类型。</param>
        /// <param name="location">扩展位置。</param>
        public ExtensionDescriptorEntry(ExtensionDescriptor descriptor, string id, string extensionType, string location)
        {
            Descriptor = descriptor;
            Id = id;
            ExtensionType = extensionType;
            Location = location;
        }

        /// <summary>
        /// 扩展描述符。
        /// </summary>
        public ExtensionDescriptor Descriptor { get; private set; }

        /// <summary>
        /// 标识。
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public string ExtensionType { get; private set; }

        /// <summary>
        /// 位置。
        /// </summary>
        public string Location { get; private set; }
    }
}