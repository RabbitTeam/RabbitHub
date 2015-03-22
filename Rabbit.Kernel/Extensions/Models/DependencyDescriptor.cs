namespace Rabbit.Kernel.Extensions.Models
{
    /// <summary>
    /// 依赖描述符。
    /// </summary>
    public sealed class DependencyDescriptor
    {
        /// <summary>
        /// 扩展Id。
        /// </summary>
        public string ExtensionId { get; set; }

        /// <summary>
        /// 扩展版本范围。
        /// </summary>
        public VersionRange Version { get; set; }
    }
}