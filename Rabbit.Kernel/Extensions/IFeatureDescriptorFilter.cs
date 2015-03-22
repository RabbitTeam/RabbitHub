using Rabbit.Kernel.Extensions.Models;

namespace Rabbit.Kernel.Extensions
{
    /// <summary>
    /// 特性描述符过滤器上下文。
    /// </summary>
    public sealed class FeatureDescriptorFilterContext
    {
        /// <summary>
        /// 初始化一个新的特性描述符过滤器上下文。
        /// </summary>
        /// <param name="feature">特性描述符。</param>
        public FeatureDescriptorFilterContext(FeatureDescriptor feature)
        {
            Feature = feature;
            Valid = true;
        }

        /// <summary>
        /// 特性描述符。
        /// </summary>
        public FeatureDescriptor Feature { get; private set; }

        /// <summary>
        /// 是否可用。
        /// </summary>
        public bool Valid { get; set; }
    }

    /// <summary>
    /// 一个抽象的特性描述符过滤器。
    /// </summary>
    public interface IFeatureDescriptorFilter
    {
        /// <summary>
        /// 在发现特性时执行。
        /// </summary>
        /// <param name="context">特性描述符过滤器上下文。</param>
        void OnDiscovery(FeatureDescriptorFilterContext context);
    }
}