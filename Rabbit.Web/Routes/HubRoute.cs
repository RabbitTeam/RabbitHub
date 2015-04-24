using Rabbit.Kernel.Environment.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rabbit.Web.Routes
{
    internal sealed class HubRoute : RouteBase, IRouteWithArea, IComparable<HubRoute>
    {
        #region Field

        private readonly IRunningShellTable _runningShellTable;

        private readonly ConcurrentDictionary<string, IList<RouteBase>> _routesByShell = new ConcurrentDictionary<string, IList<RouteBase>>();

        #endregion Field

        #region Constructor

        public HubRoute(string name, string area, int priority, IRunningShellTable runningShellTable)
        {
            Priority = priority;
            Area = area;
            Name = name;
            _runningShellTable = runningShellTable;
        }

        #endregion Constructor

        #region Property

        public string Name { get; private set; }

        public int Priority { get; private set; }

        #endregion Property

        #region Overrides of RouteBase

        /// <summary>
        /// 当在派生类中重写时，会返回有关请求的路由信息。
        /// </summary>
        /// <returns>
        /// 一个对象，包含路由定义的值（如果该路由与当前请求匹配）或 null（如果该路由与请求不匹配）。
        /// </returns>
        /// <param name="httpContext">一个对象，封装有关 HTTP 请求的信息。</param>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var settings = _runningShellTable.Match(httpContext);

            if (settings == null)
                return null;

            IList<RouteBase> routes;
            return !_routesByShell.TryGetValue(settings.Name, out routes) ? null : routes.Select(route => route.GetRouteData(httpContext)).FirstOrDefault(routeData => routeData != null);
        }

        /// <summary>
        /// 当在派生类中重写时，会检查路由是否与指定值匹配，如果匹配，则生成一个 URL，然后检索有关该路由的信息。
        /// </summary>
        /// <returns>
        /// 一个对象（包含生成的 URL 和有关路由的信息）或 null（如果路由与 <paramref name="values"/> 不匹配）。
        /// </returns>
        /// <param name="requestContext">一个对象，封装有关所请求的路由的信息。</param><param name="values">一个包含路由参数的对象。</param>
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var settings = _runningShellTable.Match(requestContext.HttpContext);

            if (settings == null)
                return null;

            IList<RouteBase> routes;
            return !_routesByShell.TryGetValue(settings.Name, out routes) ? null : routes.Select(route => route.GetVirtualPath(requestContext, values)).FirstOrDefault(virtualPathData => virtualPathData != null);
        }

        #endregion Overrides of RouteBase

        #region Implementation of IRouteWithArea

        /// <summary>
        /// Gets the name of the area to associate the route with.
        /// </summary>
        /// <returns>
        /// The name of the area to associate the route with.
        /// </returns>
        public string Area { get; private set; }

        #endregion Implementation of IRouteWithArea

        #region Implementation of IComparable<in HubRoute>

        /// <summary>
        /// 比较当前对象和同一类型的另一对象。
        /// </summary>
        /// <returns>
        /// 一个值，指示要比较的对象的相对顺序。返回值的含义如下：值含义小于零此对象小于 <paramref name="other"/> 参数。零此对象等于 <paramref name="other"/>。大于零此对象大于 <paramref name="other"/>。
        /// </returns>
        /// <param name="other">与此对象进行比较的对象。</param>
        public int CompareTo(HubRoute other)
        {
            if (other == null)
            {
                return -1;
            }

            if (other == this)
            {
                return 0;
            }

            if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(other.Name) || Name == other.Name)
            {
                return 0;
            }

            if (!string.Equals(other.Area, Area, StringComparison.OrdinalIgnoreCase))
            {
                return StringComparer.OrdinalIgnoreCase.Compare(other.Area, Area);
            }

            if (other.Priority == Priority)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(other.Area, Area);
            }

            return Priority.CompareTo(other.Priority);
        }

        #endregion Implementation of IComparable<in HubRoute>

        #region Public Method

        public void ReleaseShell(ShellSettings shellSettings)
        {
            IList<RouteBase> routes;
            _routesByShell.TryRemove(shellSettings.Name, out routes);
        }

        public void Add(RouteBase route, ShellSettings shellSettings)
        {
            var routes = _routesByShell.GetOrAdd(shellSettings.Name, key => new List<RouteBase>());
            routes.Add(route);
        }

        #endregion Public Method
    }
}