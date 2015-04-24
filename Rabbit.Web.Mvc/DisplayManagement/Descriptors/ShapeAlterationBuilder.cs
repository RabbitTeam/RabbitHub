using Rabbit.Kernel.Extensions.Models;
using Rabbit.Web.Mvc.DisplayManagement.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors
{
    /// <summary>
    /// 形状候补建造者。
    /// </summary>
    public class ShapeAlterationBuilder
    {
        private Feature _feature;
        private readonly string _shapeType;
        private readonly string _bindingName;
        private readonly IList<Action<ShapeDescriptor>> _configurations = new List<Action<ShapeDescriptor>>();

        /// <summary>
        /// 初始化一个新的形状候补建造者。
        /// </summary>
        /// <param name="feature">特性。</param>
        /// <param name="shapeType">形状类型。</param>
        public ShapeAlterationBuilder(Feature feature, string shapeType)
        {
            _feature = feature;
            _bindingName = shapeType;
            var delimiterIndex = shapeType.IndexOf("__", StringComparison.Ordinal);

            _shapeType = delimiterIndex < 0 ? shapeType : shapeType.Substring(0, delimiterIndex);
        }

        /// <summary>
        /// 来自哪一个特性。
        /// </summary>
        /// <param name="feature">特性。</param>
        /// <returns>形状候补建造者。</returns>
        public ShapeAlterationBuilder From(Feature feature)
        {
            _feature = feature;
            return this;
        }

        /// <summary>
        /// 配置。
        /// </summary>
        /// <param name="action">配置委托。</param>
        /// <returns>形状候补建造者。</returns>
        public ShapeAlterationBuilder Configure(Action<ShapeDescriptor> action)
        {
            _configurations.Add(action);
            return this;
        }

        /// <summary>
        /// 绑定配置。
        /// </summary>
        /// <param name="bindingSource">绑定源。</param>
        /// <param name="binder">绑定委托。</param>
        /// <returns>形状候补建造者。</returns>
        public ShapeAlterationBuilder BoundAs(string bindingSource, Func<ShapeDescriptor, Func<DisplayContext, IHtmlString>> binder)
        {
            return Configure(descriptor =>
            {
                Func<DisplayContext, IHtmlString> target = null;

                var binding = new ShapeBinding
                {
                    ShapeDescriptor = descriptor,
                    BindingName = _bindingName,
                    BindingSource = bindingSource,
                    Binding = displayContext =>
                    {
                        if (target == null)
                            target = binder(descriptor);

                        return target(displayContext);
                    }
                };

                descriptor.Bindings[_bindingName] = binding;
            });
        }

        /// <summary>
        /// 创建前执行。
        /// </summary>
        /// <param name="action">委托。</param>
        /// <returns>形状候补建造者。</returns>
        public ShapeAlterationBuilder OnCreating(Action<ShapeCreatingContext> action)
        {
            return Configure(descriptor =>
            {
                var existing = descriptor.Creating ?? Enumerable.Empty<Action<ShapeCreatingContext>>();
                descriptor.Creating = existing.Concat(new[] { action });
            });
        }

        /// <summary>
        /// 创建后执行。
        /// </summary>
        /// <param name="action">委托。</param>
        /// <returns>形状候补建造者。</returns>
        public ShapeAlterationBuilder OnCreated(Action<ShapeCreatedContext> action)
        {
            return Configure(descriptor =>
            {
                var existing = descriptor.Created ?? Enumerable.Empty<Action<ShapeCreatedContext>>();
                descriptor.Created = existing.Concat(new[] { action });
            });
        }

        /// <summary>
        /// 显示前执行。
        /// </summary>
        /// <param name="action">委托。</param>
        /// <returns>形状候补建造者。</returns>
        public ShapeAlterationBuilder OnDisplaying(Action<ShapeDisplayingContext> action)
        {
            return Configure(descriptor =>
            {
                var existing = descriptor.Displaying ?? Enumerable.Empty<Action<ShapeDisplayingContext>>();
                descriptor.Displaying = existing.Concat(new[] { action });
            });
        }

        /// <summary>
        /// 显示后执行。
        /// </summary>
        /// <param name="action">委托。</param>
        /// <returns>形状候补建造者。</returns>
        public ShapeAlterationBuilder OnDisplayed(Action<ShapeDisplayedContext> action)
        {
            return Configure(descriptor =>
            {
                var existing = descriptor.Displayed ?? Enumerable.Empty<Action<ShapeDisplayedContext>>();
                descriptor.Displayed = existing.Concat(new[] { action });
            });
        }

        /// <summary>
        /// 放置。
        /// </summary>
        /// <param name="action">委托。</param>
        /// <returns>形状候补建造者。</returns>
        public ShapeAlterationBuilder Placement(Func<ShapePlacementContext, PlacementInfo> action)
        {
            return Configure(descriptor =>
            {
                var next = descriptor.Placement;
                descriptor.Placement = ctx => action(ctx) ?? next(ctx);
            });
        }

        /// <summary>
        /// 放置。
        /// </summary>
        /// <param name="predicate">筛选器。</param>
        /// <param name="location">位置。</param>
        /// <returns>形状候补建造者。</returns>
        public ShapeAlterationBuilder Placement(Func<ShapePlacementContext, bool> predicate, PlacementInfo location)
        {
            return Configure(descriptor =>
            {
                var next = descriptor.Placement;
                descriptor.Placement = ctx => predicate(ctx) ? location : next(ctx);
            });
        }

        /// <summary>
        /// 生成。
        /// </summary>
        /// <returns>形状候补。</returns>
        public ShapeAlteration Build()
        {
            return new ShapeAlteration(_shapeType, _feature, _configurations.ToArray());
        }
    }

    /// <summary>
    /// 形状放置上下文。
    /// </summary>
    public class ShapePlacementContext
    {
        //        public IContent Content { get; set; }
        /// <summary>
        /// 上下文类型。
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public string Stereotype { get; set; }

        /// <summary>
        /// 显示类型。
        /// </summary>
        public string DisplayType { get; set; }

        /// <summary>
        /// 微分。
        /// </summary>
        public string Differentiator { get; set; }

        /// <summary>
        /// 路径。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 源。
        /// </summary>
        public string Source { get; set; }
    }
}