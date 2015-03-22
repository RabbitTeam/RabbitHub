namespace Rabbit.Kernel.Localization
{
    /// <summary>
    /// 一个获取本地化字符串的委托。
    /// </summary>
    /// <param name="text">文本。</param>
    /// <param name="args">参数。</param>
    public delegate LocalizedString Localizer(string text, params object[] args);
}