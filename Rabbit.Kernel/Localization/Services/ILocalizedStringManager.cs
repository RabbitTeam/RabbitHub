namespace Rabbit.Kernel.Localization.Services
{
    /// <summary>
    /// 一个抽象的本地化字符串管理者。
    /// </summary>
    public interface ILocalizedStringManager : IDependency
    {
        /// <summary>
        /// 获取本地化字符串。
        /// </summary>
        /// <param name="scope">作用范围。</param>
        /// <param name="text">文本。</param>
        /// <param name="cultureName">文化名称。</param>
        /// <returns>本地化字符串。</returns>
        string GetLocalizedString(string scope, string text, string cultureName);
    }
}