using System.IO;
using System.Text;
using System.Web;

namespace Rabbit.Web.Mvc.Mvc.Spooling
{
    /// <summary>
    /// Html字符串写入器。
    /// </summary>
    public sealed class HtmlStringWriter : TextWriter, IHtmlString
    {
        #region Field

        private readonly TextWriter _writer;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的Html字符串写入器。
        /// </summary>
        public HtmlStringWriter()
        {
            _writer = new StringWriter();
        }

        #endregion Constructor

        #region Overrides of TextWriter

        /// <summary>
        /// 将字符串写入文本流。
        /// </summary>
        /// <param name="value">要写入的字符串。</param><exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed.</exception><exception cref="T:System.IO.IOException">发生 I/O 错误。</exception>
        public override void Write(string value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// 将字符写入文本流。
        /// </summary>
        /// <param name="value">要写入文本流中的字符。</param><exception cref="T:System.ObjectDisposedException"><see cref="T:System.IO.TextWriter"/> 是关闭的。</exception><exception cref="T:System.IO.IOException">发生 I/O 错误。</exception>
        public override void Write(char value)
        {
            _writer.Write(value);
        }

        #endregion Overrides of TextWriter

        #region Overrides of Object

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString()
        {
            return _writer.ToString();
        }

        #endregion Overrides of Object

        #region Overrides of TextWriter

        /// <summary>
        /// 当在派生类中重写时，返回用来写输出的 <see cref="T:System.Text.Encoding"/>。
        /// </summary>
        /// <returns>
        /// 用来写入输出的 Encoding。
        /// </returns>
        public override Encoding Encoding
        {
            get { return _writer.Encoding; }
        }

        #endregion Overrides of TextWriter

        #region Implementation of IHtmlString

        /// <summary>
        /// 返回 HTML 编码的字符串。
        /// </summary>
        /// <returns>
        /// HTML 编码的字符串。
        /// </returns>
        public string ToHtmlString()
        {
            return _writer.ToString();
        }

        #endregion Implementation of IHtmlString
    }
}