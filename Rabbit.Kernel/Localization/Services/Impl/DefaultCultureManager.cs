using Rabbit.Kernel.Works;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Localization.Services.Impl
{
    internal sealed class DefaultCultureManager : ICultureManager
    {
        #region Constructor

        private readonly IEnumerable<ICultureSelector> _cultureSelectors;
        private readonly IWorkContextAccessor _workContextAccessor;

        #endregion Constructor

        #region Constructor

        public DefaultCultureManager(IEnumerable<ICultureSelector> cultureSelectors, IWorkContextAccessor workContextAccessor)
        {
            _cultureSelectors = cultureSelectors;
            _workContextAccessor = workContextAccessor;
        }

        #endregion Constructor

        #region Implementation of ICultureManager

        /// <summary>
        /// 获取当前文化。
        /// </summary>
        /// <param name="workContext">工作上下文。</param>
        /// <returns>文化名称。</returns>
        public string GetCurrentCulture(WorkContext workContext)
        {
            var requestCulture = _cultureSelectors
                .Select(x => x.GetCulture(workContext))
                .Where(x => x != null)
                .OrderByDescending(x => x.Priority);

            if (!requestCulture.Any())
                return string.Empty;

            foreach (var culture in requestCulture.Where(culture => !string.IsNullOrEmpty(culture.CultureName)))
            {
                return culture.CultureName;
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取租户文化。
        /// </summary>
        /// <returns>文化名称。</returns>
        public string GetTenantCulture()
        {
            return _workContextAccessor.GetContext().CurrentTenant == null ? null : _workContextAccessor.GetContext().CurrentTenant.TenantCulture;
        }

        #endregion Implementation of ICultureManager
    }
}