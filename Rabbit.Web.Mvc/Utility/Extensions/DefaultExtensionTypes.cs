namespace Rabbit.Web.Mvc.Utility.Extensions
{
    /// <summary>
    /// 默认扩展类型。
    /// </summary>
    public static class DefaultExtensionTypes
    {
        /// <summary>
        /// 主题扩展类型。
        /// </summary>
        public const string Theme = "Theme";

        /// <summary>
        /// 模块扩展类型。
        /// </summary>
        public const string Module = "Module";

        /// <summary>
        /// 是否是一个主题。
        /// </summary>
        /// <param name="type">扩展类型。</param>
        /// <returns>如果是返回true，否则返回false。</returns>
        public static bool IsTheme(string type)
        {
            return type == Theme;
        }

        /// <summary>
        /// 是否是一个模块。
        /// </summary>
        /// <param name="type">扩展类型。</param>
        /// <returns>如果是返回true，否则返回false。</returns>
        public static bool IsModule(string type)
        {
            return type == Module;
        }
    }
}