using Rabbit.Kernel;
using Rabbit.Web.Mvc.DisplayManagement.Shapes;
using System.Web;

namespace Rabbit.Web.Mvc.DisplayManagement.Implementation
{
    /// <summary>
    /// 一个抽象的形状显示事件。
    /// </summary>
    public interface IShapeDisplayEvents : IDependency
    {
        /// <summary>
        /// 显示前执行。
        /// </summary>
        /// <param name="context">形状显示前上下文。</param>
        void Displaying(ShapeDisplayingContext context);

        /// <summary>
        /// 显示完成后执行。
        /// </summary>
        /// <param name="context">形状显示后上下文。</param>
        void Displayed(ShapeDisplayedContext context);
    }

    /// <summary>
    /// 形状显示前上下文。
    /// </summary>
    public sealed class ShapeDisplayingContext
    {
        /// <summary>
        /// 形状。
        /// </summary>
        public dynamic Shape { get; set; }

        /// <summary>
        /// 形状元数据。
        /// </summary>
        public ShapeMetadata ShapeMetadata { get; set; }

        /// <summary>
        /// 子级内容。
        /// </summary>
        public IHtmlString ChildContent { get; set; }
    }

    /// <summary>
    /// 形状显示后上下文。
    /// </summary>
    public sealed class ShapeDisplayedContext
    {
        /// <summary>
        /// 形状。
        /// </summary>
        public dynamic Shape { get; set; }

        /// <summary>
        /// 形状元数据。
        /// </summary>
        public ShapeMetadata ShapeMetadata { get; set; }

        /// <summary>
        /// 子级内容。
        /// </summary>
        public IHtmlString ChildContent { get; set; }
    }

    /// <summary>
    /// 形状显示事件抽象。
    /// </summary>
    public abstract class ShapeDisplayEvents : IShapeDisplayEvents
    {
        #region Implementation of IShapeDisplayEvents

        /// <summary>
        /// 显示前执行。
        /// </summary>
        /// <param name="context">形状显示前上下文。</param>
        public virtual void Displaying(ShapeDisplayingContext context)
        {
        }

        /// <summary>
        /// 显示完成后执行。
        /// </summary>
        /// <param name="context">形状显示后上下文。</param>
        public virtual void Displayed(ShapeDisplayedContext context)
        {
        }

        #endregion Implementation of IShapeDisplayEvents
    }
}