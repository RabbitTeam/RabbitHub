using System;

namespace Rabbit.Kernel.Bus
{
    /// <summary>
    /// 消息调度员。
    /// </summary>
    public interface IMessageDispatcher : ISingletonDependency
    {
        /// <summary>
        /// 消息调度时触发的事件。
        /// </summary>
        event EventHandler<MessageDispatchEventArgs> Dispatching;

        /// <summary>
        /// 消息调度失败时触发事件。
        /// </summary>
        event EventHandler<MessageDispatchFailEventArgs> DispatchFailed;

        /// <summary>
        /// 完成消息调度触发的事件（就算调度失败也会执行）。
        /// </summary>
        event EventHandler<MessageDispatchEventArgs> Dispatched;

        /// <summary>
        /// 调度消息。
        /// </summary>
        /// <param name="message">需要被调度的消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> 为null。</exception>
        /// <exception cref="AggregateException">处理过程中一个或多个处理程序发生了异常。</exception>
        void DispatchMessage(object message);

        /// <summary>
        /// 注册消息处理到消息调度。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="messageHandler">需要注册的处理程序。</param>
        /// <returns>由系统生成的唯一Key。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="messageHandler"/> 为null。</exception>
        string Register<TMessage>(Action<TMessage> messageHandler);

        /// <summary>
        /// 从消息调度取消一个已注册的消息处理。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="key">唯一Key。</param>
        /// <exception cref="ArgumentException"><paramref name="key"/> 为空。</exception>
        void UnRegister<TMessage>(string key);

        /// <summary>
        /// 清除已登记注册的消息处理程序。
        /// </summary>
        /// <param name="isClearWaitDispatch">是否清除等待的调度处理程序。</param>
        void Clear(bool isClearWaitDispatch = false);

        /// <summary>
        /// 清除等待的调度。
        /// </summary>
        void ClearWaitDispatch();
    }
}