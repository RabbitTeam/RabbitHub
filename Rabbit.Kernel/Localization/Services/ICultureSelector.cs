using Rabbit.Kernel.Works;

namespace Rabbit.Kernel.Localization.Services
{
    /// <summary>
    /// 文化选择结果。
    /// </summary>
    public class CultureSelectorResult
    {
        /// <summary>
        /// 优先级。
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 文化名称。
        /// </summary>
        public string CultureName { get; set; }
    }

    /// <summary>
    /// 一个抽象的文化选择器。
    /// </summary>
    public interface ICultureSelector : IDependency
    {
        /// <summary>
        /// 根据工作上下文获取文化。
        /// </summary>
        /// <param name="workContext">工作上下文。</param>
        /// <returns>文化选择结果。</returns>
        CultureSelectorResult GetCulture(WorkContext workContext);
    }
}