using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Utility.Extensions;
using Rabbit.Web.Works;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace Rabbit.Web.Routes.Impl
{
    internal sealed class DefaultRoutePublisher : IRoutePublisher
    {
        #region Field

        private readonly RouteCollection _routeCollection;
        private readonly ShellSettings _shellSettings;
        private readonly IExtensionManager _extensionManager;
        private readonly IWebWorkContextAccessor _webWorkContextAccessor;
        private readonly IRunningShellTable _runningShellTable;
        private readonly IEnumerable<IRoutePublisherEventHandler> _routePublisherEventHandlers;

        #endregion Field

        #region Constructor

        public DefaultRoutePublisher(RouteCollection routeCollection, ShellSettings shellSettings, IExtensionManager extensionManager, IWebWorkContextAccessor webWorkContextAccessor, IRunningShellTable runningShellTable, IEnumerable<IRoutePublisherEventHandler> routePublisherEventHandlers)
        {
            _routeCollection = routeCollection;
            _shellSettings = shellSettings;
            _extensionManager = extensionManager;
            _webWorkContextAccessor = webWorkContextAccessor;
            _runningShellTable = runningShellTable;
            _routePublisherEventHandlers = routePublisherEventHandlers;
        }

        #endregion Constructor

        #region Implementation of IRoutePublisher

        /// <summary>
        /// 发布路由。
        /// </summary>
        /// <param name="routes">路由集合。</param>
        public void Publish(IEnumerable<RouteDescriptor> routes)
        {
            //排序。
            var routesArray = routes
                .OrderByDescending(r => r.Priority)
                .ToArray();

            //发布前事件。
            _routePublisherEventHandlers.Invoke(i => i.Publishing(routesArray), NullLogger.Instance);

            using (_routeCollection.GetWriteLock())
            {
                //释放现有路由。
                _routeCollection
                    .OfType<HubRoute>().Invoke(x => x.ReleaseShell(_shellSettings), NullLogger.Instance);
                var routeList = new List<RouteBase>(_routeCollection);

                //添加新路由
                foreach (var routeDescriptor in routesArray)
                {
                    //根据Route得到扩展描述符
                    ExtensionDescriptorEntry extensionDescriptor = null;
                    if (routeDescriptor.Route is Route)
                    {
                        object extensionId;
                        var route = routeDescriptor.Route as Route;
                        if (route.DataTokens != null && route.DataTokens.TryGetValue("area", out extensionId) ||
                            route.Defaults != null && route.Defaults.TryGetValue("area", out extensionId))
                        {
                            extensionDescriptor = _extensionManager.GetExtension(extensionId.ToString());
                        }
                    }
                    else if (routeDescriptor.Route is IRouteWithArea)
                    {
                        var route = routeDescriptor.Route as IRouteWithArea;
                        extensionDescriptor = _extensionManager.GetExtension(route.Area);
                    }

                    //加载会话状态信息。
                    var sessionState = SessionStateBehavior.Default;
                    if (extensionDescriptor != null)
                    {
                        if (routeDescriptor.SessionState == SessionStateBehavior.Default)
                        {
                            var descriptor = extensionDescriptor.Descriptor;
                            if (descriptor.Keys.Contains("SessionState"))
                                Enum.TryParse(descriptor["SessionState"], true, out sessionState);
                        }
                    }

                    //设置SessionState
                    var sessionStateBehavior = routeDescriptor.SessionState == SessionStateBehavior.Default
                        ? sessionState
                        : routeDescriptor.SessionState;

                    //创建外壳路由
                    var shellRoute = new ShellRoute(routeDescriptor.Route, _shellSettings, _webWorkContextAccessor,
                        _runningShellTable)
                    {
                        IsHttpRoute = routeDescriptor is HttpRouteDescriptor,
                        SessionState = sessionStateBehavior
                    };

                    //区域
                    var area = extensionDescriptor == null ? string.Empty : extensionDescriptor.Id;

                    //尝试查找已存在的集线器路由
                    var matchedHubRoute = routeList.FirstOrDefault(x =>
                    {
                        var hubRoute = x as HubRoute;
                        if (hubRoute == null)
                        {
                            return false;
                        }

                        return routeDescriptor.Priority == hubRoute.Priority &&
                               hubRoute.Area.Equals(area, StringComparison.OrdinalIgnoreCase) &&
                               hubRoute.Name == routeDescriptor.Name;
                    }) as HubRoute;

                    //创建新的集线器路由。
                    if (matchedHubRoute == null)
                    {
                        matchedHubRoute = new HubRoute(routeDescriptor.Name, area, routeDescriptor.Priority,
                            _runningShellTable);

                        int index;
                        for (index = 0; index < routeList.Count; index++)
                        {
                            var hubRoute = routeList[index] as HubRoute;
                            if (hubRoute == null)
                            {
                                continue;
                            }
                            if (hubRoute.Priority < matchedHubRoute.Priority)
                            {
                                break;
                            }
                        }
                        routeList.Insert(index, matchedHubRoute);
                    }

                    matchedHubRoute.Add(shellRoute, _shellSettings);
                }

                //清空现有路由。
                _routeCollection.Clear();
                foreach (var item in routeList)
                {
                    if (item is HubRoute)
                    {
                        _routeCollection.Add((item as HubRoute).Name, item);
                    }
                    else
                    {
                        _routeCollection.Add(item);
                    }
                }
            }

            //发布后事件。
            _routePublisherEventHandlers.Invoke(i => i.Published(routesArray), NullLogger.Instance);
        }

        #endregion Implementation of IRoutePublisher
    }
}