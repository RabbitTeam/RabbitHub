using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Web.Environment.Extensions;
using Rabbit.Web.Works;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace Rabbit.Web.Routes
{
    internal sealed class ShellRoute : RouteBase, IRouteWithArea
    {
        #region Field

        private readonly RouteBase _route;
        private readonly ShellSettings _shellSettings;
        private readonly IWebWorkContextAccessor _workContextAccessor;
        private readonly IRunningShellTable _runningShellTable;
        private readonly UrlPrefix _urlPrefix;

        #endregion Field

        #region Property

        public bool IsHttpRoute { get; set; }

        public SessionStateBehavior SessionState { get; set; }

        #endregion Property

        #region Constructor

        public ShellRoute(RouteBase route, ShellSettings shellSettings, IWebWorkContextAccessor workContextAccessor, IRunningShellTable runningShellTable)
        {
            _route = route;
            _shellSettings = shellSettings;
            _runningShellTable = runningShellTable;
            _workContextAccessor = workContextAccessor;
            if (!string.IsNullOrEmpty(_shellSettings.GetRequestUrlPrefix()))
                _urlPrefix = new UrlPrefix(_shellSettings.GetRequestUrlPrefix());

            Area = route.GetAreaName();
        }

        #endregion Constructor

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
            //根据Http上下文定位具体的租户外壳设置。
            var settings = _runningShellTable.Match(httpContext);

            if (settings == null || settings.Name != _shellSettings.Name)
                return null;

            var effectiveHttpContext = httpContext;
            if (_urlPrefix != null)
                effectiveHttpContext = new UrlPrefixAdjustedHttpContext(httpContext, _urlPrefix);

            var routeData = _route.GetRouteData(effectiveHttpContext);
            if (routeData == null)
                return null;

            routeData.RouteHandler = new RouteHandler(_workContextAccessor, routeData.RouteHandler, SessionState);
            routeData.DataTokens["IWorkContextAccessor"] = _workContextAccessor;

            if (IsHttpRoute)
            {
                routeData.Values["IWorkContextAccessor"] = _workContextAccessor; // for WebApi
            }

            return routeData;
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
            //根据Http上下文定位具体的租户外壳设置。
            var settings = _runningShellTable.Match(requestContext.HttpContext);

            if (settings == null || settings.Name != _shellSettings.Name)
                return null;

            var effectiveRequestContext = requestContext;
            if (_urlPrefix != null)
                effectiveRequestContext = new RequestContext(new UrlPrefixAdjustedHttpContext(requestContext.HttpContext, _urlPrefix), requestContext.RouteData);

            var virtualPath = _route.GetVirtualPath(effectiveRequestContext, values);
            if (virtualPath == null)
                return null;

            if (_urlPrefix != null)
                virtualPath.VirtualPath = _urlPrefix.PrependLeadingSegments(virtualPath.VirtualPath);

            return virtualPath;
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

        #region Help Class

        private class RouteHandler : IRouteHandler
        {
            private readonly IWebWorkContextAccessor _workContextAccessor;
            private readonly IRouteHandler _routeHandler;
            private readonly SessionStateBehavior _sessionStateBehavior;

            public RouteHandler(IWebWorkContextAccessor workContextAccessor, IRouteHandler routeHandler, SessionStateBehavior sessionStateBehavior)
            {
                _workContextAccessor = workContextAccessor;
                _routeHandler = routeHandler;
                _sessionStateBehavior = sessionStateBehavior;
            }

            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                var httpHandler = _routeHandler.GetHttpHandler(requestContext);
                requestContext.HttpContext.SetSessionStateBehavior(_sessionStateBehavior);

                if (httpHandler is IHttpAsyncHandler)
                    return new HttpAsyncHandler(_workContextAccessor, httpHandler as IHttpAsyncHandler);
                return new HttpHandler(_workContextAccessor, httpHandler);
            }
        }

        private class HttpHandler : IHttpHandler, IRequiresSessionState, IHasRequestContext
        {
            protected readonly IWebWorkContextAccessor WorkContextAccessor;
            private readonly IHttpHandler _httpHandler;

            public HttpHandler(IWebWorkContextAccessor workContextAccessor, IHttpHandler httpHandler)
            {
                WorkContextAccessor = workContextAccessor;
                _httpHandler = httpHandler;
            }

            public bool IsReusable
            {
                get { return false; }
            }

            public void ProcessRequest(HttpContext context)
            {
                using (WorkContextAccessor.CreateWorkContextScope(new HttpContextWrapper(context)))
                {
                    _httpHandler.ProcessRequest(context);
                }
            }

            public RequestContext RequestContext
            {
                get
                {
                    var mvcHandler = _httpHandler as MvcHandler;
                    return mvcHandler == null ? null : mvcHandler.RequestContext;
                }
            }
        }

        private class HttpAsyncHandler : HttpHandler, IHttpAsyncHandler
        {
            private readonly IHttpAsyncHandler _httpAsyncHandler;
            private IDisposable _scope;

            public HttpAsyncHandler(IWebWorkContextAccessor containerProvider, IHttpAsyncHandler httpAsyncHandler)
                : base(containerProvider, httpAsyncHandler)
            {
                _httpAsyncHandler = httpAsyncHandler;
            }

            public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
            {
                _scope = WorkContextAccessor.CreateWorkContextScope(new HttpContextWrapper(context));
                try
                {
                    return _httpAsyncHandler.BeginProcessRequest(context, cb, extraData);
                }
                catch
                {
                    _scope.Dispose();
                    throw;
                }
            }

            public void EndProcessRequest(IAsyncResult result)
            {
                try
                {
                    _httpAsyncHandler.EndProcessRequest(result);
                }
                finally
                {
                    _scope.Dispose();
                }
            }
        }

        #endregion Help Class
    }
}