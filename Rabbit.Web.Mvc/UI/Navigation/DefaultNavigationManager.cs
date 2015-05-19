using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Logging;
using Rabbit.Web.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MenuItem = Rabbit.Web.UI.Navigation.MenuItem;

namespace Rabbit.Web.Mvc.UI.Navigation
{
    internal sealed class DefaultNavigationManager : INavigationManager
    {
        private readonly Lazy<IEnumerable<INavigationProvider>> _navigationProviders;
        private readonly UrlHelper _urlHelper;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        public DefaultNavigationManager(Lazy<IEnumerable<INavigationProvider>> navigationProviders, UrlHelper urlHelper, ICacheManager cacheManager, ISignals signals)
        {
            _navigationProviders = navigationProviders;
            _urlHelper = urlHelper;
            _cacheManager = cacheManager;
            _signals = signals;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        #region Implementation of INavigationManager

        /// <summary>
        /// 生成菜单。
        /// </summary>
        /// <param name="menuName">菜单名称。</param>
        /// <returns>菜单项集合。</returns>
        public IEnumerable<MenuItem> BuildMenu(string menuName)
        {
            var sources = GetSources(menuName);
            const bool hasDebugShowAllMenuItems = true;
            return FinishMenu(Reduce(Merge(sources), hasDebugShowAllMenuItems).ToArray());
        }

        /// <summary>
        /// 生成图片集。
        /// </summary>
        /// <param name="menuName">菜单名称。</param>
        /// <returns>图片集。</returns>
        public IEnumerable<string> BuildImageSets(string menuName)
        {
            return GetImageSets(menuName).SelectMany(imageSets => imageSets.Distinct()).Distinct();
        }

        /// <summary>
        /// 获取菜单Url。
        /// </summary>
        /// <param name="menuItemUrl">菜单项Url。</param>
        /// <param name="routeValueDictionary">路由值。</param>
        /// <returns>Url。</returns>
        public string GetUrl(string menuItemUrl, RouteValueDictionary routeValueDictionary)
        {
            var url = string.IsNullOrEmpty(menuItemUrl) && (routeValueDictionary == null || routeValueDictionary.Count == 0)
                          ? "~/"
                          : !string.IsNullOrEmpty(menuItemUrl)
                                ? menuItemUrl
                                : RouteUrl(routeValueDictionary);

            //忽略非法的Url地址。
            if (string.IsNullOrEmpty(url) || _urlHelper.RequestContext.HttpContext == null ||
                (url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("/")))
                return url;

            if (url.StartsWith("~/"))
            {
                url = url.Substring(2);
            }
            if (url.StartsWith("#"))
                return url;

            var appPath = _urlHelper.RequestContext.HttpContext.Request.ApplicationPath;
            if (appPath == "/")
                appPath = string.Empty;
            url = string.Format("{0}/{1}", appPath, url);
            return url;
        }

        #endregion Implementation of INavigationManager

        #region Private Method

        #region RouteUrl

        //TODO:因发现MVC中的BUG，暂时模拟MVC内部进行Url获取
        private string RouteUrl(RouteValueDictionary routeValueDictionary)
        {
            var text = GenerateUrl(_urlHelper.RouteCollection, routeValueDictionary);
            return text;
        }

        private string GenerateUrl(RouteCollection routeCollection, RouteValueDictionary routeValueDictionary)
        {
            var requestContext = _urlHelper.RequestContext;
            var virtualPathForArea = routeCollection.GetVirtualPath(requestContext, null,
                routeValueDictionary ?? new RouteValueDictionary());
            return virtualPathForArea == null ? null : GenerateClientUrl(requestContext.HttpContext, virtualPathForArea.VirtualPath);
        }

        private static string GenerateClientUrl(HttpContextBase httpContext, string contentPath)
        {
            if (string.IsNullOrEmpty(contentPath))
            {
                return contentPath;
            }
            string str;
            contentPath = StripQuery(contentPath, out str);
            return GenerateClientUrlInternal(httpContext, contentPath) + str;
        }

        private static string StripQuery(string path, out string query)
        {
            var num = path.IndexOf('?');
            if (num >= 0)
            {
                query = path.Substring(num);
                return path.Substring(0, num);
            }
            query = null;
            return path;
        }

        private static readonly UrlRewriterHelper _urlRewriterHelper = new UrlRewriterHelper();

        private static string GenerateClientUrlInternal(HttpContextBase httpContext, string contentPath)
        {
            if (string.IsNullOrEmpty(contentPath))
            {
                return contentPath;
            }
            var flag = contentPath[0] == '~';
            if (flag)
            {
                var virtualPath = VirtualPathUtility.ToAbsolute(contentPath, httpContext.Request.ApplicationPath);
                var contentPath2 = httpContext.Response.ApplyAppPathModifier(virtualPath);
                return GenerateClientUrlInternal(httpContext, contentPath2);
            }
            if (!_urlRewriterHelper.WasRequestRewritten(httpContext))
            {
                return contentPath;
            }
            var relativePath = MakeRelative(httpContext.Request.Path, contentPath);
            return MakeAbsolute(httpContext.Request.RawUrl, relativePath);
        }

        public static string MakeRelative(string fromPath, string toPath)
        {
            var text = VirtualPathUtility.MakeRelative(fromPath, toPath);
            if (string.IsNullOrEmpty(text) || text[0] == '?')
            {
                text = "./" + text;
            }
            return text;
        }

        public static string MakeAbsolute(string basePath, string relativePath)
        {
            string text;
            basePath = StripQuery(basePath, out text);
            return VirtualPathUtility.Combine(basePath, relativePath);
        }

        private sealed class UrlRewriterHelper
        {
            private readonly object _lockObject = new object();
            private bool _urlRewriterIsTurnedOnValue;
            private volatile bool _urlRewriterIsTurnedOnCalculated;

            private static bool WasThisRequestRewritten(HttpContextBase httpContext)
            {
                var serverVariables = httpContext.Request.ServerVariables;
                return serverVariables != null && serverVariables.AllKeys.Contains("IIS_WasUrlRewritten");
            }

            private bool IsUrlRewriterTurnedOn(HttpContextBase httpContext)
            {
                if (_urlRewriterIsTurnedOnCalculated)
                    return _urlRewriterIsTurnedOnValue;
                lock (_lockObject)
                {
                    if (_urlRewriterIsTurnedOnCalculated)
                        return _urlRewriterIsTurnedOnValue;
                    var serverVariables = httpContext.Request.ServerVariables;
                    try
                    {
                        var urlRewriterIsTurnedOnValue = serverVariables != null && serverVariables.AllKeys.Contains("IIS_UrlRewriteModule");
                        _urlRewriterIsTurnedOnValue = urlRewriterIsTurnedOnValue;
                        _urlRewriterIsTurnedOnCalculated = true;
                    }
                    catch
                    {
                    }
                }
                return _urlRewriterIsTurnedOnValue;
            }

            public bool WasRequestRewritten(HttpContextBase httpContext)
            {
                return IsUrlRewriterTurnedOn(httpContext) && WasThisRequestRewritten(httpContext);
            }
        }

        #endregion RouteUrl

        private IEnumerable<MenuItem> FinishMenu(IEnumerable<MenuItem> menuItems)
        {
            menuItems = menuItems.ToArray();
            foreach (var menuItem in menuItems)
            {
                menuItem.Href = GetUrl(menuItem.Url, menuItem.RouteValues);
                menuItem.Items = FinishMenu(menuItem.Items.ToArray());
            }

            return menuItems;
        }

        private static IEnumerable<MenuItem> Reduce(IEnumerable<MenuItem> items, bool hasDebugShowAllMenuItems)
        {
            foreach (var item in items.Where(item =>
                hasDebugShowAllMenuItems))
            {
                item.Items = Reduce(item.Items, hasDebugShowAllMenuItems);
                yield return item;
            }
        }

        private IEnumerable<IEnumerable<MenuItem>> GetSources(string menuName)
        {
            return GetNavigationBuilders(menuName).Select(i => i.Build());
        }

        private IEnumerable<IEnumerable<string>> GetImageSets(string menuName)
        {
            return GetNavigationBuilders(menuName).Select(navigationBuilder => navigationBuilder.BuildImageSets());
        }

        private IEnumerable<NavigationBuilder> GetNavigationBuilders(string menuName)
        {
            var list = new List<NavigationBuilder>();

            foreach (var provider in _navigationProviders.Value.Where(i => i.MenuName == menuName))
            {
                var providerProxy = provider;
                var navigationBuilder = _cacheManager.Get(provider.GetType().FullName, context =>
                {
                    context.Monitor(
                        _signals.When("Rabbit.Web.Mvc.Navigation." + providerProxy.GetType().FullName + ".Change"));
                    var builder = new NavigationBuilder();
                    try
                    {
                        providerProxy.GetNavigation(builder);
                    }
                    catch (Exception ex)
                    {
                        builder = null;
                        Logger.Error(ex, "意外的错误在查询导航提供商。它被忽略。由供应商提供的菜单可能不完整。");
                    }
                    return builder;
                });
                if (navigationBuilder != null)
                    list.Add(navigationBuilder);
            }

            return list.ToArray();
        }

        private static IEnumerable<MenuItem> Merge(IEnumerable<IEnumerable<MenuItem>> sources)
        {
            var comparer = new MenuItemComparer();
            var orderer = new FlatPositionComparer();

            return sources.SelectMany(x => x).ToArray()
                //相同的菜单
                .GroupBy(key => key, (key, items) => Join(items), comparer)
                //相同的位置
                .GroupBy(item => item.Position)
                .OrderBy(positionGroup => positionGroup.Key, orderer)
                .SelectMany(positionGroup => positionGroup.OrderBy(item => item.Text == null ? string.Empty : item.Text.TextHint));
        }

        private static MenuItem Join(IEnumerable<MenuItem> items)
        {
            var list = items.ToArray();

            if (list.Count() < 2)
                return list.Single();

            var joined = new MenuItem
            {
                Text = list.First().Text,
                Classes = list.Select(x => x.Classes).FirstOrDefault(x => x != null && x.Count > 0),
                Url = list.Select(x => x.Url).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)),
                Href = list.Select(x => x.Href).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)),
                RouteValues = list.Select(x => x.RouteValues).FirstOrDefault(x => x != null),
                LocalNavigation = list.Any(x => x.LocalNavigation),
                Position = SelectBestPositionValue(list.Select(x => x.Position)),
                Items = Merge(list.Select(x => x.Items)).ToArray()
            };

            return joined;
        }

        private static string SelectBestPositionValue(IEnumerable<string> positions)
        {
            var comparer = new FlatPositionComparer();
            return positions.Aggregate(string.Empty,
                                       (agg, pos) =>
                                       string.IsNullOrEmpty(agg)
                                           ? pos
                                           : string.IsNullOrEmpty(pos)
                                                 ? agg
                                                 : comparer.Compare(agg, pos) < 0 ? agg : pos);
        }

        #endregion Private Method
    }
}