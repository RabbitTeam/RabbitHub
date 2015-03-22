using Rabbit.Kernel.Works;
using System;

namespace Rabbit.Kernel.Settings.Impl
{
    internal sealed class CurrentTenantWorkContext : IWorkContextStateProvider
    {
        #region Field

        private readonly ITenantService _tenantService;

        #endregion Field

        #region Constructor

        public CurrentTenantWorkContext(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        #endregion Constructor

        #region Implementation of IWorkContextStateProvider

        /// <summary>
        /// 创建一个从一个工作上下文获取服务的委托。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <param name="name">服务名称。</param>
        /// <returns>从一个工作上下文获取服务的委托。</returns>
        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name != "CurrentTenant") return null;
            var siteSettings = _tenantService.GetTenantSettings();
            return ctx => (T)siteSettings;
        }

        #endregion Implementation of IWorkContextStateProvider
    }
}