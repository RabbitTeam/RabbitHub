using Rabbit.Web.Mvc.DisplayManagement.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rabbit.Web.Mvc.DisplayManagement.Shapes
{
    /// <summary>
    /// 形状元数据。
    /// </summary>
    public sealed class ShapeMetadata
    {
        /// <summary>
        /// 初始化一个新的形状元数据。
        /// </summary>
        public ShapeMetadata()
        {
            Wrappers = new List<string>();
            Alternates = new List<string>();
            BindingSources = new List<string>();
            Displaying = Enumerable.Empty<Action<ShapeDisplayingContext>>();
            Displayed = Enumerable.Empty<Action<ShapeDisplayedContext>>();
        }

        /// <summary>
        /// 类型。
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 显示类型。
        /// </summary>
        public string DisplayType { get; set; }

        /// <summary>
        /// 位置。
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 放置源。
        /// </summary>
        public string PlacementSource { get; set; }

        /// <summary>
        /// 前缀。
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 包装。
        /// </summary>
        public IList<string> Wrappers { get; set; }

        /// <summary>
        /// 候补。
        /// </summary>
        public IList<string> Alternates { get; set; }

        /// <summary>
        /// 是否有执行完成。
        /// </summary>
        public bool WasExecuted { get; set; }

        /// <summary>
        /// 子内容。
        /// </summary>
        public IHtmlString ChildContent { get; set; }

        /// <summary>
        /// 显示前委托集合。
        /// </summary>
        public IEnumerable<Action<ShapeDisplayingContext>> Displaying { get; private set; }

        /// <summary>
        /// 显示后委托集合。
        /// </summary>
        public IEnumerable<Action<ShapeDisplayedContext>> Displayed { get; private set; }

        /// <summary>
        /// 绑定源。
        /// </summary>
        public IList<string> BindingSources { get; set; }

        /// <summary>
        /// 在显示前时执行委托。
        /// </summary>
        /// <param name="action">委托。</param>
        public void OnDisplaying(Action<ShapeDisplayingContext> action)
        {
            var existing = Displaying ?? Enumerable.Empty<Action<ShapeDisplayingContext>>();
            Displaying = existing.Concat(new[] { action });
        }

        /// <summary>
        /// 在显示完成后执行委托。
        /// </summary>
        /// <param name="action">委托。</param>
        public void OnDisplayed(Action<ShapeDisplayedContext> action)
        {
            var existing = Displayed ?? Enumerable.Empty<Action<ShapeDisplayedContext>>();
            Displayed = existing.Concat(new[] { action });
        }
    }
}