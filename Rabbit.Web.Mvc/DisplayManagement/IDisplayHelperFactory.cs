using Rabbit.Kernel;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.DisplayManagement
{
    /// <summary>
    /// 一个抽象的显示助手工厂。
    /// </summary>
    public interface IDisplayHelperFactory : IDependency
    {
        /// <summary>
        /// 创建助手。
        /// </summary>
        /// <param name="viewContext">视图上下文。</param>
        /// <param name="viewDataContainer">视图数据容器。</param>
        /// <returns>显示助手。</returns>
        dynamic CreateHelper(ViewContext viewContext, IViewDataContainer viewDataContainer);
    }
}