using Rabbit.Web.Mvc.DisplayManagement.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors
{
    /// <summary>
    /// 形状描述符。
    /// </summary>
    public sealed class ShapeDescriptor
    {
        /// <summary>
        /// 初始化一个新的形状描述符。
        /// </summary>
        public ShapeDescriptor()
        {
            Creating = Enumerable.Empty<Action<ShapeCreatingContext>>();
            Created = Enumerable.Empty<Action<ShapeCreatedContext>>();
            Displaying = Enumerable.Empty<Action<ShapeDisplayingContext>>();
            Displayed = Enumerable.Empty<Action<ShapeDisplayedContext>>();
            Wrappers = new List<string>();
            BindingSources = new List<string>();
            Bindings = new Dictionary<string, ShapeBinding>(StringComparer.OrdinalIgnoreCase);
            Placement = ctx => new PlacementInfo { Location = DefaultPlacement };
        }

        /// <summary>
        /// 形状类型。
        /// </summary>
        public string ShapeType { get; set; }

        /// <summary>
        /// 绑定源。
        /// </summary>
        public string BindingSource
        {
            get
            {
                ShapeBinding binding;
                return Bindings.TryGetValue(ShapeType, out binding) ? binding.BindingSource : null;
            }
        }

        /// <summary>
        /// 绑定委托。
        /// </summary>
        public Func<DisplayContext, IHtmlString> Binding
        {
            get
            {
                return Bindings[ShapeType].Binding;
            }
        }

        /// <summary>
        /// 绑定字典表。
        /// </summary>
        public IDictionary<string, ShapeBinding> Bindings { get; set; }

        /// <summary>
        /// 创建前委托集合。
        /// </summary>
        public IEnumerable<Action<ShapeCreatingContext>> Creating { get; set; }

        /// <summary>
        /// 创建完成委托集合。
        /// </summary>
        public IEnumerable<Action<ShapeCreatedContext>> Created { get; set; }

        /// <summary>
        /// 显示前委托集合。
        /// </summary>
        public IEnumerable<Action<ShapeDisplayingContext>> Displaying { get; set; }

        /// <summary>
        /// 显示完成委托集合。
        /// </summary>
        public IEnumerable<Action<ShapeDisplayedContext>> Displayed { get; set; }

        /// <summary>
        /// 放置委托。
        /// </summary>
        public Func<ShapePlacementContext, PlacementInfo> Placement { get; set; }

        /// <summary>
        /// 默认放置。
        /// </summary>
        public string DefaultPlacement { get; set; }

        /// <summary>
        /// 包装集合。
        /// </summary>
        public IList<string> Wrappers { get; set; }

        /// <summary>
        /// 绑定源集合。
        /// </summary>
        public IList<string> BindingSources { get; set; }
    }

    /// <summary>
    /// 形状绑定信息。
    /// </summary>
    public sealed class ShapeBinding
    {
        /// <summary>
        /// 形状描述符。
        /// </summary>
        public ShapeDescriptor ShapeDescriptor { get; set; }

        /// <summary>
        /// 绑定名称。
        /// </summary>
        public string BindingName { get; set; }

        /// <summary>
        /// 绑定源。
        /// </summary>
        public string BindingSource { get; set; }

        /// <summary>
        /// 绑定委托。
        /// </summary>
        public Func<DisplayContext, IHtmlString> Binding { get; set; }
    }
}