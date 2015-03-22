namespace Rabbit.Kernel.Settings
{
    /// <summary>
    /// 一个抽象的租户服务。
    /// </summary>
    public interface ITenantService : IDependency
    {
        /// <summary>
        /// 获取当前租户设置。
        /// </summary>
        /// <returns>当前租户设置实例。</returns>
        ITenant GetTenantSettings();
    }
}