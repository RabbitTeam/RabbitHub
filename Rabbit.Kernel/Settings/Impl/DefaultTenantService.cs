using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Environment.Configuration;

namespace Rabbit.Kernel.Settings.Impl
{
    internal sealed class DefaultTenantService : ITenantService
    {
        #region Field

        private readonly ShellSettings _shellSettings;
        private readonly ICacheManager _cacheManager;

        #endregion Field

        #region Constructor

        public DefaultTenantService(ShellSettings shellSettings, ICacheManager cacheManager)
        {
            _shellSettings = shellSettings;
            _cacheManager = cacheManager;
        }

        #endregion Constructor

        #region Implementation of ITenantService

        /// <summary>
        /// 获取当前租户设置。
        /// </summary>
        /// <returns>当前租户设置实例。</returns>
        public ITenant GetTenantSettings()
        {
            return _cacheManager.Get("Site", ctx => new DefaultTenant(_shellSettings.Name));
        }

        #endregion Implementation of ITenantService
    }
}