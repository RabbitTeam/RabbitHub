using Rabbit.Kernel;
using System.Web;

namespace Rabbit.Web.Mvc.DisplayManagement.Implementation
{
    /// <summary>
    /// 一个抽象的显示管理者。
    /// </summary>
    public interface IDisplayManager : IDependency
    {
        /// <summary>
        /// 执行。
        /// </summary>
        /// <param name="context">显示上下文。</param>
        /// <returns>Html字符串实例。</returns>
        IHtmlString Execute(DisplayContext context);
    }
}