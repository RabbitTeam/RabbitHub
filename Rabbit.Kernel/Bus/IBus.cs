using Rabbit.Kernel.Bus.Configurators;
using Rabbit.Kernel.Bus.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rabbit.Kernel.Bus
{
    /// <summary>
    /// 表示一个抽象总线。
    /// </summary>
    public interface IBus
    {
        /// <summary>
        /// 发布一个消息到总线。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="message">需要发布的消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> 为null。</exception>
        void Publish<TMessage>(TMessage message);

        /// <summary>
        /// 订阅一个消息。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="messageHandler">消息处理程序。</param>
        /// <exception cref="ArgumentNullException"><paramref name="messageHandler"/> 为null。</exception>
        /// <returns>由系统生成的唯一Key。</returns>
        string Subscribe<TMessage>(Action<TMessage> messageHandler);

        /// <summary>
        /// 清除等待发布的消息。
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 总线基类。
    /// </summary>
    public abstract class Bus : IBus
    {
        private readonly IMessageDispatcher _messageDispatcher;

        /// <summary>
        /// 初始化一个新的Bug。
        /// </summary>
        /// <param name="messageDispatcher">消息调度员。</param>
        /// <exception cref="ArgumentNullException"><paramref name="messageDispatcher"/> 为 null。</exception>
        protected Bus(IMessageDispatcher messageDispatcher)
        {
            if (messageDispatcher == null)
                throw new ArgumentNullException("messageDispatcher");

            _messageDispatcher = messageDispatcher;
        }

        #region Implementation of IBus

        /// <summary>
        /// 发布一个消息到总线。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="message">需要发布的消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> 为null。</exception>
        void IBus.Publish<TMessage>(TMessage message)
        {
            SetPublishDateTime(message);
            Publish(message);
        }

        /// <summary>
        /// 订阅一个消息。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="messageHandler">消息处理程序。</param>
        /// <exception cref="ArgumentNullException"><paramref name="messageHandler"/> 为null。</exception>
        /// <returns>由系统生成的唯一Key。</returns>
        string IBus.Subscribe<TMessage>(Action<TMessage> messageHandler)
        {
            return Subscribe(messageHandler);
        }

        /// <summary>
        /// 清除等待发布的消息。
        /// </summary>
        void IBus.Clear()
        {
            Clear();
        }

        #endregion Implementation of IBus

        #region Protected Virtual Method

        /// <summary>
        /// 发布一个消息到总线。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="message">需要发布的消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> 为null。</exception>
        protected virtual void Publish<TMessage>(TMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            _messageDispatcher.DispatchMessage(message);
        }

        /// <summary>
        /// 订阅一个消息。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="messageHandler">消息处理程序。</param>
        /// <exception cref="ArgumentNullException"><paramref name="messageHandler"/> 为null。</exception>
        /// <returns>由系统生成的唯一Key。</returns>
        protected virtual string Subscribe<TMessage>(Action<TMessage> messageHandler)
        {
            if (messageHandler == null)
                throw new ArgumentNullException("messageHandler");

            return _messageDispatcher.Register(messageHandler);
        }

        /// <summary>
        /// 清除等待发布的消息。
        /// </summary>
        protected virtual void Clear()
        {
            _messageDispatcher.ClearWaitDispatch();
        }

        #endregion Protected Virtual Method

        #region Private Method

        /// <summary>
        /// 设置消息发布时间。
        /// </summary>
        /// <param name="message">消息实例。</param>
        private static void SetPublishDateTime(object message)
        {
            if (message is MessageBase)
                (message as MessageBase).PublishDateTimeUtc = DateTime.UtcNow;
        }

        #endregion Private Method
    }

    /// <summary>
    /// 总线扩展。
    /// </summary>
    public static class BusExtensions
    {
        /// <summary>
        /// 发布一个消息集合到总线。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="bus">总线。</param>
        /// <param name="message">需要发布的消息。</param>
        /// <param name="isStart">是否立即启动任务。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="message"/> 为null。</exception>
        /// <returns>一个发布任务。</returns>
        public static Task PublishAsync<TMessage>(this IBus bus, TMessage message, bool isStart = true)
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (message == null)
                throw new ArgumentNullException("message");

            var task = new Task(() => bus.Publish(message));
            if (isStart)
                task.Start();
            return task;
        }

        /// <summary>
        /// 发布一个消息集合到总线。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="bus">总线。</param>
        /// <param name="messages">需要发布的消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="messages"/> 为null。</exception>
        public static void Publishs<TMessage>(this IBus bus, IEnumerable<TMessage> messages)
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (messages == null)
                throw new ArgumentNullException("messages");

            foreach (var message in messages)
                bus.Publish(message);
        }

        /// <summary>
        /// 异步发布一个消息集合到总线。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="bus">总线。</param>
        /// <param name="messages">需要发布的消息。</param>
        /// <param name="isStart">是否立即启动任务。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="messages"/> 为null。</exception>
        /// <returns>一个发布任务。</returns>
        public static Task PublishsAsync<TMessage>(this IBus bus, IEnumerable<TMessage> messages, bool isStart = true)
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (messages == null)
                throw new ArgumentNullException("messages");

            var task = new Task(() =>
            {
                var result = Parallel.ForEach(messages, bus.Publish);
                while (!result.IsCompleted)
                    Thread.Sleep(100);
            });
            if (isStart)
                task.Start();
            return task;
        }

        /// <summary>
        /// 发布一个请求。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="bus">总线。</param>
        /// <param name="message">消息。</param>
        /// <param name="configurator">请求配置。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="message"/> 或 <paramref name="configurator"/> 为null。</exception>
        /// <exception cref="RequestTimeoutException">请求超时。</exception>
        /// <returns>是否成功。</returns>
        public static void PublishRequest<TMessage>(this IBus bus, TMessage message, Action<RequestConfigurator> configurator) where TMessage : MessageBase
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (message == null)
                throw new ArgumentNullException("message");
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            //配置请求。
            configurator(new RequestConfigurator(bus, message));
            bus.Publish(message);
        }

        /// <summary>
        /// 发布一个请求，如果请求在10秒之内没有任何回应则中断该请求。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="bus">总线。</param>
        /// <param name="message">消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="message"/> 为null。</exception>
        /// <exception cref="RequestTimeoutException">请求超时。</exception>
        /// <returns>是否成功。</returns>
        public static object PublishRequest<TMessage>(this IBus bus, TMessage message) where TMessage : MessageBase
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (message == null)
                throw new ArgumentNullException("message");

            return bus.PublishRequest(message, configurator => configurator.Timeout(new TimeSpan(0, 0, 10)));
        }

        /// <summary>
        /// 发布一个请求。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="bus">总线。</param>
        /// <param name="message">消息。</param>
        /// <param name="configurator">请求超时相关的配置动作。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="message"/> 为null。</exception>
        /// <exception cref="RequestTimeoutException">请求超时。</exception>
        /// <returns>是否成功。</returns>
        public static object PublishRequest<TMessage>(this IBus bus, TMessage message, Action<TimeoutConfigurator> configurator) where TMessage : MessageBase
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (message == null)
                throw new ArgumentNullException("message");
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            //初始化超时配置。
            var timeoutConfigurator = new TimeoutConfigurator();
            configurator(timeoutConfigurator);

            //任务起始时间。
            var startDateTime = DateTime.Now;
            //同步锁。
            var autoResetEvent = new AutoResetEvent(false);
            object result = null;
            //开始请求。
            bus.PublishRequest(message, c => c.Handle(responseMessage =>
            {
                try
                {
                    var exception = responseMessage.Exception;
                    if (exception != null)
                        throw new Exception("请求接收方处理程序发生了异常。", exception);
                    result = responseMessage.Result;
                }
                finally
                {
                    //取消线程锁定。
                    autoResetEvent.Set();
                }
            }));

            //等待线程。
            autoResetEvent.WaitOne(timeoutConfigurator.TimeoutTime);

            if (DateTime.Now.Subtract(startDateTime) <= timeoutConfigurator.TimeoutTime)
                return result;

            //如果超时则抛出超时异常。
            var timeoutException = new RequestTimeoutException(message);
            if (timeoutConfigurator.TimeoutAction == null)
                return result;
            if (!timeoutConfigurator.TimeoutAction(timeoutException))
                throw timeoutException;

            return result;
        }

        /// <summary>
        /// 发布一个请求，如果请求在10秒之内没有任何回应则中断该请求。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <typeparam name="TResult">返回结果的类型。</typeparam>
        /// <param name="bus">总线。</param>
        /// <param name="message">消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="message"/> 为null。</exception>
        /// <exception cref="RequestTimeoutException">请求超时。</exception>
        /// <returns>是否成功。</returns>
        public static TResult PublishRequest<TMessage, TResult>(this IBus bus, TMessage message) where TMessage : MessageBase
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (message == null)
                throw new ArgumentNullException("message");

            return (TResult)bus.PublishRequest(message, configurator => configurator.Timeout(new TimeSpan(0, 0, 10)));
        }

        /// <summary>
        /// 发布一个请求。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <typeparam name="TResult">返回结果的类型。</typeparam>
        /// <param name="bus">总线。</param>
        /// <param name="message">消息。</param>
        /// <param name="configurator">请求超时相关的配置动作。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="message"/> 为null。</exception>
        /// <exception cref="RequestTimeoutException">请求超时。</exception>
        /// <returns>是否成功。</returns>
        public static TResult PublishRequest<TMessage, TResult>(this IBus bus, TMessage message, Action<TimeoutConfigurator> configurator) where TMessage : MessageBase
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (message == null)
                throw new ArgumentNullException("message");
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            return (TResult)bus.PublishRequest(message, configurator);
        }

        /// <summary>
        /// 订阅一个消息。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="bus">总线。</param>
        /// <param name="handler">处理程序。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="handler"/> 或 <paramref name="handler"/> 为null。</exception>
        public static void Subscribe<TMessage, TResult>(this IBus bus, Func<TMessage, TResult> handler) where TMessage : MessageBase
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (handler == null)
                throw new ArgumentNullException("handler");
            bus.Subscribe<TMessage>((message, configurator) =>
            {
                object result = null;
                try
                {
                    result = handler(message);
                    configurator.Post(result);
                }
                catch (Exception exception)
                {
                    configurator.Post(result, exception);
                    throw;
                }
            });
        }

        /// <summary>
        /// 订阅一个消息。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="bus">总线。</param>
        /// <param name="handler">处理程序。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="handler"/> 或 <paramref name="handler"/> 为null。</exception>
        public static void Subscribe<TMessage>(this IBus bus, Action<TMessage, ResponseConfigurator> handler) where TMessage : MessageBase
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (handler == null)
                throw new ArgumentNullException("handler");
            bus.Subscribe<TMessage>(message => handler(message, new ResponseConfigurator(bus, message)));
        }
    }
}