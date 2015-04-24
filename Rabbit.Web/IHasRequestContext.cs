using System.Web.Routing;

namespace Rabbit.Web
{
    /// <summary>
    /// 表示含有一个请求上下文的抽象接口。
    /// </summary>
    public interface IHasRequestContext
    {
        /// <summary>
        /// 请求上下文。
        /// </summary>
        RequestContext RequestContext { get; }
    }
}