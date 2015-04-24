using Rabbit.Kernel;
using System;

namespace Rabbit.Web.Mvc.DisplayManagement
{
    /// <summary>
    /// 一个抽象的形状工厂。
    /// </summary>
    public interface IShapeFactory : IDependency
    {
        /// <summary>
        /// 创建形状。
        /// </summary>
        /// <param name="shapeType">形状类型。</param>
        /// <returns>形状实例。</returns>
        IShape Create(string shapeType);

        /// <summary>
        /// 创建形状。
        /// </summary>
        /// <param name="shapeType">形状类型。</param>
        /// <param name="parameters">参数。</param>
        /// <returns>形状实例。</returns>
        IShape Create(string shapeType, INamedEnumerable<object> parameters);

        /// <summary>
        /// 创建形状。
        /// </summary>
        /// <param name="shapeType">形状类型。</param>
        /// <param name="parameters">参数。</param>
        /// <param name="createShape">创建形状委托。</param>
        /// <returns>形状实例。</returns>
        IShape Create(string shapeType, INamedEnumerable<object> parameters, Func<dynamic> createShape);
    }
}