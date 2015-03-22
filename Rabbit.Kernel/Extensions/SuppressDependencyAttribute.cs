using System;

namespace Rabbit.Kernel.Extensions
{
    /// <summary>
    /// 替换依赖标记。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class SuppressDependencyAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个新的替换依赖标记。
        /// </summary>
        /// <param name="fullName">替换的类型名称。</param>
        public SuppressDependencyAttribute(string fullName)
        {
            FullName = fullName;
        }

        /// <summary>
        /// 替换的类型名称。
        /// </summary>
        public string FullName { get; set; }
    }
}