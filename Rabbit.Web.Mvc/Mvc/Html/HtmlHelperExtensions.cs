using Rabbit.Kernel.Works;
using Rabbit.Web.Works;
using System;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.Html
{
    /// <summary>
    /// Html助手扩展方法。
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// 获取工作上下文。
        /// </summary>
        /// <param name="html">Html助手。</param>
        /// <returns>工作上下文。</returns>
        public static WorkContext GetWorkContext(this HtmlHelper html)
        {
            var workContext = html.ViewContext.RequestContext.GetWorkContext();

            if (workContext == null)
                throw new ApplicationException("在请求中无法找到工作上下文。");

            return workContext;
        }
    }
}