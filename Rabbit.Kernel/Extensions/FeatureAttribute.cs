using Rabbit.Kernel.Utility.Extensions;
using System;

namespace Rabbit.Kernel.Extensions
{
    /// <summary>
    /// 特性标记。
    /// </summary>

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FeatureAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个新的特性标记。
        /// </summary>
        /// <param name="featureName">特性名称。</param>
        public FeatureAttribute(string featureName)
        {
            FeatureName = featureName.NotEmptyOrWhiteSpace("name");
        }

        /// <summary>
        /// 特性名称。
        /// </summary>
        public string FeatureName { get; private set; }
    }
}