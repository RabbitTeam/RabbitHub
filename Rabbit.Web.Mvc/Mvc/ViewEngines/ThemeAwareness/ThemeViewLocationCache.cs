using System;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.ThemeAwareness
{
    internal sealed class ThemeViewLocationCache : IViewLocationCache
    {
        private readonly string _requestTheme;

        public ThemeViewLocationCache(string requestTheme)
        {
            _requestTheme = requestTheme;
        }

        #region Implementation of IViewLocationCache

        /// <summary>
        /// Gets the view location by using the specified HTTP context and the cache key.
        /// </summary>
        /// <returns>
        /// The view location.
        /// </returns>
        /// <param name="httpContext">The HTTP context.</param><param name="key">The cache key.</param>
        public string GetViewLocation(HttpContextBase httpContext, string key)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            return (string)httpContext.Cache[AlterKey(key)];
        }

        /// <summary>
        /// Inserts the specified view location into the cache by using the specified HTTP context and the cache key.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param><param name="key">The cache key.</param><param name="virtualPath">The virtual path.</param>
        public void InsertViewLocation(HttpContextBase httpContext, string key, string virtualPath)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            httpContext.Cache.Insert(AlterKey(key), virtualPath, new CacheDependency(HostingEnvironment.MapPath("~/Themes")));
        }

        #endregion Implementation of IViewLocationCache

        #region Private Method

        private string AlterKey(string key)
        {
            return key + ":" + _requestTheme;
        }

        #endregion Private Method
    }
}