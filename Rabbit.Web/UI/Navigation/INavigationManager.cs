using Rabbit.Kernel;
using System.Collections.Generic;

namespace Rabbit.Web.UI.Navigation
{
    /// <summary>
    /// 一个抽象的导航管理者。
    /// </summary>
    public interface INavigationManager : IDependency
    {
        /// <summary>
        /// 生成菜单。
        /// </summary>
        /// <param name="menuName">菜单名称。</param>
        /// <returns>菜单项集合。</returns>
        IEnumerable<MenuItem> BuildMenu(string menuName);

        /// <summary>
        /// 生成图片集。
        /// </summary>
        /// <param name="menuName">菜单名称。</param>
        /// <returns>图片集。</returns>
        IEnumerable<string> BuildImageSets(string menuName);
    }
}