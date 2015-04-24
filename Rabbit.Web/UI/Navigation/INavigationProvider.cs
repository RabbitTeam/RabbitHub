using Rabbit.Kernel;

namespace Rabbit.Web.UI.Navigation
{
    /// <summary>
    /// 一个抽象的导航提供者。
    /// </summary>
    public interface INavigationProvider : IDependency
    {
        /// <summary>
        /// 导航菜单名称。
        /// </summary>
        string MenuName { get; }

        /// <summary>
        /// 获取导航。
        /// </summary>
        /// <param name="builder">导航建造者。</param>
        void GetNavigation(NavigationBuilder builder);
    }
}