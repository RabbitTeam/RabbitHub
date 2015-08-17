using Rabbit.Kernel.Utility.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rabbit.Kernel.Bus.Impl
{
    internal sealed class DefaultMessageDispatcher : IMessageDispatcher, IDisposable
    {
        #region Field

        private readonly CancellationTokenSource _dispatchTaskToken = new CancellationTokenSource();
        private readonly ConcurrentDictionary<string, MessageHandlerItem> _messageHandlers = new ConcurrentDictionary<string, MessageHandlerItem>();

        #endregion Field

        #region Implementation of IMessageDispatcher

        public event EventHandler<MessageDispatchEventArgs> Dispatching;

        public event EventHandler<MessageDispatchFailEventArgs> DispatchFailed;

        public event EventHandler<MessageDispatchEventArgs> Dispatched;

        /// <summary>
        /// 调度消息。
        /// </summary>
        /// <param name="message">需要被调度的消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> 为null。</exception>
        /// <exception cref="AggregateException">处理过程中一个或多个处理程序发生了异常。</exception>
        public void DispatchMessage(object message)
        {
            message.NotNull("message");

            //调度前事件。
            if (Dispatching != null)
                Dispatching(this, new MessageDispatchEventArgs(message));

            var exceptions = new List<Exception>();
            try
            {
                var token = _dispatchTaskToken.Token;
                var messageType = message.GetType();

                Parallel.ForEach(_messageHandlers.Where(i => i.Value.MessageType == messageType), (item, state) =>
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            state.Stop();
                        if (state.IsStopped)
                            return;
                        item.Value.Handle(message);
                    }
                    catch (Exception exception)
                    {
                        exceptions.Add(exception);
                        //调度失败事件。
                        if (DispatchFailed != null)
                            DispatchFailed(this, new MessageDispatchFailEventArgs(message, exception));
                    }
                });
            }
            finally
            {
                //调度完成事件。
                if (Dispatched != null)
                    Dispatched(this, new MessageDispatchEventArgs(message));
                if (exceptions.Any())
                    throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// 注册消息处理到消息调度。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="messageHandler">需要注册的处理程序。</param>
        /// <returns>由系统生成的唯一Key。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="messageHandler"/> 为null。</exception>
        public string Register<TMessage>(Action<TMessage> messageHandler)
        {
            if (messageHandler == null)
                throw new ArgumentNullException("messageHandler");

            var handlerKey = Guid.NewGuid().ToString("N");

            Func<MessageHandlerItem> getHandler = () =>
                new MessageHandlerItem(message => messageHandler((TMessage)message), typeof(TMessage));

            _messageHandlers.AddOrUpdate(handlerKey, key => getHandler(), (key, value) => getHandler());
            return handlerKey;
        }

        /// <summary>
        /// 从消息调度取消一个已注册的消息处理。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="key">唯一Key。</param>
        /// <exception cref="ArgumentException"><paramref name="key"/> 为空。</exception>
        public void UnRegister<TMessage>(string key)
        {
            key.NotEmptyOrWhiteSpace("key");

            if (!_messageHandlers.ContainsKey(key))
                return;
            MessageHandlerItem item;
            _messageHandlers.TryRemove(key, out item);
        }

        /// <summary>
        /// 清除已登记注册的消息处理程序。
        /// </summary>
        /// <param name="isClearWaitDispatch">是否清除等待的调度处理程序。</param>
        public void Clear(bool isClearWaitDispatch = false)
        {
            _messageHandlers.Clear();
            if (isClearWaitDispatch)
                ClearWaitDispatch();
        }

        /// <summary>
        /// 清除等待的调度。
        /// </summary>
        public void ClearWaitDispatch()
        {
            if (_dispatchTaskToken == null)
                return;
            using (_dispatchTaskToken)
            {
                _dispatchTaskToken.Cancel();
            }
        }

        #endregion Implementation of IMessageDispatcher

        #region Help Class

        internal sealed class MessageHandlerItem
        {
            public MessageHandlerItem(Action<object> handle, Type messageType)
            {
                MessageType = messageType;
                Handle = handle;
            }

            public Action<object> Handle { get; private set; }

            public Type MessageType { get; private set; }
        }

        #endregion Help Class

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            ClearWaitDispatch();
        }

        #endregion Implementation of IDisposable
    }
}