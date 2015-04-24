using Rabbit.Kernel.Works;
using System;

namespace Rabbit.Web.Impl
{
    internal sealed class HttpContextWorkContext : IWorkContextStateProvider
    {
        #region Field

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion Field

        #region Constructor

        public HttpContextWorkContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion Constructor

        #region Implementation of IWorkContextStateProvider

        /// <summary>
        /// 获取状态信值。
        /// </summary>
        /// <typeparam name="T">状态类型。</typeparam>
        /// <param name="name">状态名称。</param>
        /// <returns>获取状态值的委托。</returns>
        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name != "HttpContext")
                return null;
            var result = (T)(object)_httpContextAccessor.Current();
            return ctx => result;
        }

        #endregion Implementation of IWorkContextStateProvider
    }
}