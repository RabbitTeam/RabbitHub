using System;
using System.Web;

namespace Rabbit.Kernel.Localization
{
    /// <summary>
    /// 本地化字符串。
    /// </summary>
    public sealed class LocalizedString : MarshalByRefObject, IHtmlString
    {
        private readonly string _localized;
        private readonly string _scope;
        private readonly string _textHint;
        private readonly object[] _args;

        /// <summary>
        /// 初始化一个新的本地化字符串。
        /// </summary>
        /// <param name="languageNeutral">通用语言。</param>
        public LocalizedString(string languageNeutral)
        {
            _localized = languageNeutral;
            _textHint = languageNeutral;
        }

        /// <summary>
        /// 初始化一个新的本地化字符串。
        /// </summary>
        /// <param name="localized">本地化字符串。</param>
        /// <param name="scope">作用范围。</param>
        /// <param name="textHint">文本示意。</param>
        /// <param name="args">参数。</param>
        public LocalizedString(string localized, string scope, string textHint, object[] args)
        {
            _localized = localized;
            _scope = scope;
            _textHint = textHint;
            _args = args;
        }

        /// <summary>
        /// 获取一个本地化字符串，如果不存在则返回 <paramref name="defaultValue"/>。
        /// </summary>
        /// <param name="text">问吧。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>本地化字符串。</returns>
        public static LocalizedString TextOrDefault(string text, LocalizedString defaultValue)
        {
            return string.IsNullOrEmpty(text) ? defaultValue : new LocalizedString(text);
        }

        /// <summary>
        /// 本地化文本的作用范围。
        /// </summary>
        public string Scope
        {
            get { return _scope; }
        }

        /// <summary>
        /// 文本示意。
        /// </summary>
        public string TextHint
        {
            get { return _textHint; }
        }

        /// <summary>
        /// 参数。
        /// </summary>
        public object[] Args
        {
            get { return _args; }
        }

        /// <summary>
        /// 本地化字符串。
        /// </summary>
        public string Text
        {
            get { return _localized; }
        }

        /// <summary>
        /// 返回 HTML 编码的字符串。
        /// </summary>
        /// <returns>
        /// HTML 编码的字符串。
        /// </returns>
        public string ToHtmlString()
        {
            return _localized;
        }

        #region Overrides of Object

        /// <summary>
        /// 得到本地化字符串。
        /// </summary>
        /// <returns>本地化字符串。</returns>
        public override string ToString()
        {
            return _localized;
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns>
        /// 当前 <see cref="T:System.Object"/> 的哈希代码。
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = 0;
            if (_localized != null)
                hashCode ^= _localized.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// 确定指定的 <see cref="T:System.Object"/> 是否等于当前的 <see cref="T:System.Object"/>。
        /// </summary>
        /// <returns>
        /// 如果指定的 <see cref="T:System.Object"/> 等于当前的 <see cref="T:System.Object"/>，则为 true；否则为 false。
        /// </returns>
        /// <param name="obj">与当前的 <see cref="T:System.Object"/> 进行比较的 <see cref="T:System.Object"/>。</param>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var that = (LocalizedString)obj;
            return string.Equals(_localized, that._localized);
        }

        #endregion Overrides of Object
    }
}