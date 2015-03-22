using System;

namespace Rabbit.Kernel.Bus.Exceptions
{
    /// <summary>
    /// 请求超时异常。
    /// </summary>
    [Serializable]
    public sealed class RequestTimeoutException : ApplicationException
    {
        /// <summary>
        /// 初始化一个新的请求超时异常。
        /// </summary>
        /// <param name="message">消息。</param>
        public RequestTimeoutException(MessageBase message)
            : base(string.Format("Bus请求超时，{0}。", message))
        { }
    }
}