using System.Web.Routing;
using System.Web.SessionState;

namespace Rabbit.Web.Routes
{
    /// <summary>
    /// 路由描述符。
    /// </summary>
    public class RouteDescriptor
    {
        /// <summary>
        /// 路由名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 优先级。
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 路由项。
        /// </summary>
        public RouteBase Route { get; set; }

        /// <summary>
        /// 会话状态。
        /// </summary>
        public SessionStateBehavior SessionState { get; set; }
    }

    /// <summary>
    /// WebApi路由描述符。
    /// </summary>
    public sealed class HttpRouteDescriptor : RouteDescriptor
    {
        /// <summary>
        /// 路由模板。
        /// </summary>
        public string RouteTemplate { get; set; }

        /// <summary>
        /// 默认值。
        /// </summary>
        public object Defaults { get; set; }

        /// <summary>
        /// 约束。
        /// </summary>
        public object Constraints { get; set; }
    }
}