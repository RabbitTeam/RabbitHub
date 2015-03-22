using System;

namespace Rabbit.Components.Data.DataAnnotations
{
    /// <summary>
    /// 依赖倒转属性标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class InversePropertyAttribute : Attribute
    {
        /// <summary>
        /// 属性名称。
        /// </summary>
        public string Property { get; private set; }

        /// <summary>
        /// 初始化一个新的依赖倒转属性标记。
        /// </summary>
        /// <param name="property">属性名称。</param>
        public InversePropertyAttribute(string property)
        {
            if (string.IsNullOrWhiteSpace(property))
                throw new ArgumentNullException("property");
            Property = property;
        }
    }
}