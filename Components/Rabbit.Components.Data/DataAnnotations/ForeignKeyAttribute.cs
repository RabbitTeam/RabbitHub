using System;

namespace Rabbit.Components.Data.DataAnnotations
{
    /// <summary>
    /// 外键标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ForeignKeyAttribute : Attribute
    {
        /// <summary>
        /// 外键名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 初始化一个新的外键标记。
        /// </summary>
        /// <param name="name">外键名称。</param>
        public ForeignKeyAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            Name = name;
        }
    }
}