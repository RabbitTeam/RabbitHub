using System;

namespace Rabbit.Kernel.Settings
{
    /// <summary>
    /// 通过接口模式提供租户设置模型。
    /// </summary>
    public interface ITenant
    {
        /// <summary>
        /// 租户名称。
        /// </summary>
        string TenantName { get; }

        /// <summary>
        /// 超级用户。
        /// </summary>
        string SuperUser { get; }

        /// <summary>
        /// 租户文化。
        /// </summary>
        string TenantCulture { get; set; }

        /// <summary>
        /// 租户时区。
        /// </summary>
        string TenantTimeZone { get; }

        /// <summary>
        /// 根据Key获取自定义参数。
        /// </summary>
        /// <param name="key">参数Key。</param>
        /// <returns>参数值。</returns>
        /// <exception cref="ArgumentNullException">参数 <paramref name="key"/> 为空。</exception>
        object this[string key] { get; set; }
    }
}