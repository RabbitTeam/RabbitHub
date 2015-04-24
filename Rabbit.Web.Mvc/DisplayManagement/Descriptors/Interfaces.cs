using Rabbit.Kernel;
using Rabbit.Kernel.Events;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors
{
    /// <summary>
    /// 一个抽象的形状表格管理者。
    /// </summary>
    public interface IShapeTableManager : ISingletonDependency
    {
        /// <summary>
        /// 获取形状表格。
        /// </summary>
        /// <param name="themeName">主题名称。</param>
        /// <returns>形状表格。</returns>
        ShapeTable GetShapeTable(string themeName);
    }

    /// <summary>
    /// 一个抽象的形状表格提供程序。
    /// </summary>
    public interface IShapeTableProvider : IDependency
    {
        /// <summary>
        /// 发现形状表格。
        /// </summary>
        /// <param name="builder">形状表格建造者。</param>
        void Discover(ShapeTableBuilder builder);
    }

    /// <summary>
    /// 一个抽象的形状表格事件处理程序。
    /// </summary>
    public interface IShapeTableEventHandler : IEventHandler
    {
        /// <summary>
        /// 形状表格被创建时执行。
        /// </summary>
        /// <param name="shapeTable">形状表格。</param>
        void ShapeTableCreated(ShapeTable shapeTable);
    }
}