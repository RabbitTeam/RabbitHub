using System;

namespace Rabbit.Web.Mvc.Themes
{
    /// <summary>
    /// 主题标记。
    /// </summary>
    public sealed class ThemedAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个新的主题标记。
        /// </summary>
        public ThemedAttribute()
        {
            Enabled = true;
        }

        /// <summary>
        /// 初始化一个新的主题标记。
        /// </summary>
        /// <param name="enabled">是否启用主题。</param>
        public ThemedAttribute(bool enabled)
        {
            Enabled = enabled;
        }

        /// <summary>
        /// 是否启用主题。
        /// </summary>
        public bool Enabled { get; set; }
    }
}