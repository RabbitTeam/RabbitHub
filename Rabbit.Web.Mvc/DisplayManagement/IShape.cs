using Rabbit.Web.Mvc.DisplayManagement.Shapes;

namespace Rabbit.Web.Mvc.DisplayManagement
{
    /// <summary>
    /// 一个抽象的形状。
    /// </summary>
    public interface IShape
    {
        /// <summary>
        /// 形状元数据。
        /// </summary>
        ShapeMetadata Metadata { get; set; }
    }
}