using Rabbit.Kernel.Works;
using System.Web;

namespace Rabbit.Web.Works
{
    /// <summary>
    /// 一个抽象的Web上下文访问者。
    /// </summary>
    public interface IWebWorkContextAccessor : IWorkContextAccessor
    {
        /// <summary>
        /// 获取工作上下文。
        /// </summary>
        /// <param name="httpContext">Http上下文。</param>
        /// <returns>工作上下文。</returns>
        WorkContext GetContext(HttpContextBase httpContext);

        /// <summary>
        /// 创建工作上下文范围。
        /// </summary>
        /// <param name="httpContext">Http上下文。</param>
        /// <returns>工作上下文范围。</returns>
        IWorkContextScope CreateWorkContextScope(HttpContextBase httpContext);
    }
}