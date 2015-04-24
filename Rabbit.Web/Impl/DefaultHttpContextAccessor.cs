using System;
using System.Web;

namespace Rabbit.Web.Impl
{
    internal sealed class DefaultHttpContextAccessor : IHttpContextAccessor
    {
        private HttpContextBase _stub;

        #region Implementation of IHttpContextAccessor

        /// <summary>
        /// 当前Http上下文。
        /// </summary>
        /// <returns>Http上下文。</returns>
        public HttpContextBase Current()
        {
            var httpContext = GetStaticProperty();
            return httpContext != null ? new HttpContextWrapper(httpContext) : _stub;
        }

        /// <summary>
        /// 设置Http上下文。
        /// </summary>
        /// <param name="stub">存根。</param>
        public void Set(HttpContextBase stub)
        {
            _stub = stub;
        }

        #endregion Implementation of IHttpContextAccessor

        #region Private Method

        private static HttpContext GetStaticProperty()
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                return null;
            }

            try
            {
                if (httpContext.Request == null)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return httpContext;
        }

        #endregion Private Method
    }
}