namespace Rabbit.Kernel.Localization
{
    /// <summary>
    /// 一个抽象的文本。
    /// </summary>
    public interface IText
    {
        /// <summary>
        /// 获取本地化字符串。
        /// </summary>
        /// <param name="textHint">文本示意。</param>
        /// <param name="args">参数。</param>
        /// <returns>本地化字符串。</returns>
        LocalizedString Get(string textHint, params object[] args);
    }
}