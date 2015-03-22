using System;

namespace Rabbit.Kernel.Bus
{
    /// <summary>
    ///表示调度消息时所产生的事件数据。
    /// </summary>
    public class MessageDispatchEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化一个新的消息调度事件数据。
        /// </summary>
        /// <param name="message">消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> 为 null。</exception>
        public MessageDispatchEventArgs(object message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Message = message;
        }

        /// <summary>
        /// 消息。
        /// </summary>
        public object Message { get; private set; }
    }

    /// <summary>
    /// 消息调度失败时候所产生的事件数据。
    /// </summary>
    public class MessageDispatchFailEventArgs : MessageDispatchEventArgs
    {
        /// <summary>
        /// 初始化一个新的消息调度失败事件数据。
        /// </summary>
        /// <param name="message">消息。</param>
        /// <param name="exception">异常。</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> 或 <paramref name="exception"/> 为 null。</exception>
        public MessageDispatchFailEventArgs(object message, Exception exception)
            : base(message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (exception == null)
                throw new ArgumentNullException("exception");

            Exception = exception;
        }

        /// <summary>
        /// 发生的异常。
        /// </summary>
        public Exception Exception { get; private set; }
    }
}