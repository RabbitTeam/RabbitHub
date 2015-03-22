using Rabbit.Kernel.Works;

namespace Rabbit.Kernel.Localization.Services
{
    /// <summary>
    /// 一个抽象的文化管理者。
    /// </summary>
    public interface ICultureManager : IDependency
    {
        /// <summary>
        /// 获取当前文化。
        /// </summary>
        /// <param name="workContext">工作上下文。</param>
        /// <returns>文化名称。</returns>
        string GetCurrentCulture(WorkContext workContext);

        /// <summary>
        /// 获取租户文化。
        /// </summary>
        /// <returns>文化名称。</returns>
        string GetTenantCulture();
    }
}