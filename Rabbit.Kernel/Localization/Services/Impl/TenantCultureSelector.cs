using Rabbit.Kernel.Works;

namespace Rabbit.Kernel.Localization.Services.Impl
{
    internal sealed class TenantCultureSelector : ICultureSelector
    {
        #region Field

        private readonly IWorkContextAccessor _workContextAccessor;

        #endregion Field

        #region Constructor

        public TenantCultureSelector(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        #endregion Constructor

        #region Implementation of ICultureSelector

        /// <summary>
        /// 根据工作上下文获取文化。
        /// </summary>
        /// <param name="workContext">工作上下文。</param>
        /// <returns>文化选择结果。</returns>
        public CultureSelectorResult GetCulture(WorkContext workContext)
        {
            var currentCultureName = _workContextAccessor.GetContext().CurrentTenant.TenantCulture;

            return string.IsNullOrEmpty(currentCultureName) ? null : new CultureSelectorResult { Priority = -5, CultureName = currentCultureName };
        }

        #endregion Implementation of ICultureSelector
    }
}