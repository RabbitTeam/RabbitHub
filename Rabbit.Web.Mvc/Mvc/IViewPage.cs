using Rabbit.Kernel.Localization;
using Rabbit.Web.Mvc.Works;
using System;
using System.Web;

namespace Rabbit.Web.Mvc.Mvc
{
    /// <summary>
    /// 一个抽象的视图页。
    /// </summary>
    public interface IViewPage
    {
        /// <summary>
        /// 本地化委托。
        /// </summary>
        Localizer T { get; }

        /// <summary>
        /// 显示形状。
        /// </summary>
        dynamic Display { get; }

        /// <summary>
        /// 布局页。
        /// </summary>
        dynamic Layout { get; }

        /// <summary>
        /// 显示子级内容。
        /// </summary>
        /// <param name="shape">形状。</param>
        /// <returns>Html字符串。</returns>
        IHtmlString DisplayChildren(object shape);

        /// <summary>
        /// 当前工作上下文。
        /// </summary>
        MvcWorkContext WorkContext { get; }

        /// <summary>
        /// 捕获。
        /// </summary>
        /// <param name="callback">委托。</param>
        /// <returns>可释放的对象。</returns>
        IDisposable Capture(Action<IHtmlString> callback);

        /// <summary>
        /// 是否存在文本。
        /// </summary>
        /// <param name="thing">对象。</param>
        /// <returns>如果是则返回true，否则返回false。</returns>
        bool HasText(object thing);
    }
}