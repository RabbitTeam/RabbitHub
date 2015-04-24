using Rabbit.Kernel;
using Rabbit.Web.Mvc.DisplayManagement.Shapes;
using System.Collections.Generic;

namespace Rabbit.Web.Mvc.DisplayManagement
{
    /// <summary>
    /// 一个抽象的形状显示。
    /// </summary>
    public interface IShapeDisplay : IDependency
    {
        /// <summary>
        /// 显示。
        /// </summary>
        /// <param name="shape">形状。</param>
        /// <returns>结果。</returns>
        string Display(Shape shape);

        /// <summary>
        /// 显示。
        /// </summary>
        /// <param name="shape">形状。</param>
        /// <returns>结果。</returns>
        string Display(object shape);

        /// <summary>
        /// 显示。
        /// </summary>
        /// <param name="shapes">形状集合。</param>
        /// <returns>结果集合。</returns>
        IEnumerable<string> Display(IEnumerable<object> shapes);
    }
}