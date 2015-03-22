using Rabbit.Kernel.Works;
using System;

namespace Rabbit.Kernel.Localization.Services.Impl
{
    internal sealed class CurrentCultureWorkContext : IWorkContextStateProvider
    {
        #region Field

        private readonly ICultureManager _cultureManager;

        #endregion Field

        #region Constructor

        public CurrentCultureWorkContext(ICultureManager cultureManager)
        {
            _cultureManager = cultureManager;
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
            if (name == "CurrentCulture")
            {
                return ctx => (T)(object)_cultureManager.GetCurrentCulture(ctx);
            }
            return null;
        }

        #endregion Implementation of IWorkContextStateProvider
    }
}