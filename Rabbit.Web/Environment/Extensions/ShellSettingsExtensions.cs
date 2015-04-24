using Rabbit.Kernel.Environment.Configuration;

namespace Rabbit.Web.Environment.Extensions
{
    /// <summary>
    /// 外壳设置扩展方法。
    /// </summary>
    public static class ShellSettingsExtensions
    {
        /// <summary>
        /// 获取请求Url前缀。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        /// <returns>请求Url前缀。</returns>
        public static string GetRequestUrlPrefix(this ShellSettings settings)
        {
            return settings["RequestUrlPrefix"];
        }

        /// <summary>
        /// 获取请求Url主机名称。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        /// <returns>请求Url主机名称。</returns>
        public static string GetRequestUrlHost(this ShellSettings settings)
        {
            return settings["RequestUrlHost"];
        }

        /// <summary>
        /// 设置请求Url主机名称。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        /// <param name="host">主机名称。</param>
        public static void SetRequestUrlHost(this ShellSettings settings, string host)
        {
            settings["RequestUrlHost"] = host;
        }
    }
}