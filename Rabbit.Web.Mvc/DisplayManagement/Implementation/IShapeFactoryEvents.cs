using Rabbit.Kernel;
using System;
using System.Collections.Generic;

namespace Rabbit.Web.Mvc.DisplayManagement.Implementation
{
    /// <summary>
    /// 形状工厂事件。
    /// </summary>
    public interface IShapeFactoryEvents : IDependency
    {
        /// <summary>
        /// 创建前执行。
        /// </summary>
        /// <param name="context">形状创建前上下文。</param>
        void Creating(ShapeCreatingContext context);

        /// <summary>
        /// 创建完成后执行。
        /// </summary>
        /// <param name="context">形状创建完成上下文。</param>
        void Created(ShapeCreatedContext context);
    }

    /// <summary>
    /// 形状创建前上下文。
    /// </summary>
    public class ShapeCreatingContext
    {
        /// <summary>
        /// 形状工厂。
        /// </summary>
        public IShapeFactory ShapeFactory { get; set; }

        /// <summary>
        /// 形状工厂。
        /// </summary>
        public dynamic New { get; set; }

        /// <summary>
        /// 形状类型。
        /// </summary>
        public string ShapeType { get; set; }

        /// <summary>
        /// 创建委托。
        /// </summary>
        public Func<dynamic> Create { get; set; }

        /// <summary>
        /// 创建完成之后执行。
        /// </summary>
        public IList<Action<ShapeCreatedContext>> OnCreated { get; set; }
    }

    /// <summary>
    /// 形状创建完成上下文。
    /// </summary>
    public class ShapeCreatedContext
    {
        /// <summary>
        /// 形状工厂。
        /// </summary>
        public IShapeFactory ShapeFactory { get; set; }

        /// <summary>
        /// 形状工厂。
        /// </summary>
        public dynamic New { get; set; }

        /// <summary>
        /// 形状类型。
        /// </summary>
        public string ShapeType { get; set; }

        /// <summary>
        /// 形状。
        /// </summary>
        public dynamic Shape { get; set; }
    }

    /// <summary>
    /// 形状工厂时间抽象。
    /// </summary>
    public abstract class ShapeFactoryEvents : IShapeFactoryEvents
    {
        #region Implementation of IShapeFactoryEvents

        /// <summary>
        /// 创建前执行。
        /// </summary>
        /// <param name="context">形状创建前上下文。</param>
        public virtual void Creating(ShapeCreatingContext context)
        {
        }

        /// <summary>
        /// 创建完成后执行。
        /// </summary>
        /// <param name="context">形状创建完成上下文。</param>
        public virtual void Created(ShapeCreatedContext context)
        {
        }

        #endregion Implementation of IShapeFactoryEvents
    }
}