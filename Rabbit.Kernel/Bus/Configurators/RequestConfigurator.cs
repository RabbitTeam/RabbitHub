using System;

namespace Rabbit.Kernel.Bus.Configurators
{
    /// <summary>
    /// 请求配置。
    /// </summary>
    public sealed class RequestConfigurator
    {
        /// <summary>
        /// 对应的总线。
        /// </summary>
        private readonly IBus _bus;

        private readonly MessageBase _message;

        /// <summary>
        /// 初始化一个新的请求配置。
        /// </summary>
        /// <param name="bus">总线。</param>
        /// <param name="message">消息实例。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="message"/> 为null。</exception>
        public RequestConfigurator(IBus bus, MessageBase message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            _bus = bus;
            _message = message;
        }

        /// <summary>
        /// 为该请求配置一个处理程序。
        /// </summary>
        /// <param name="handler">处理程序。</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> 为 null。</exception>
        /// <returns>请求配置。</returns>
        public RequestConfigurator Handle(Action<MessageBase, ResponseMessage> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return Handle(responseMessage => handler(_message, responseMessage));
        }

        /// <summary>
        /// 为该请求配置一个处理程序。
        /// </summary>
        /// <param name="handler">处理程序。</param>
        /// <exception cref="ArgumentNullException"><paramref name="handler"/> 为 null。</exception>
        /// <returns>请求配置。</returns>
        public RequestConfigurator Handle(Action<ResponseMessage> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _bus.Subscribe<ResponseMessage>(responseMessage =>
            {
                //如果请求消息的Id不等于当前配置的消息Id则忽略。
                if (responseMessage.RequestMessageId != _message.Id)
                    return;
                handler(responseMessage);
            });

            return this;
        }
    }

    /// <summary>
    /// 请求配置扩展。
    /// </summary>
    public static class RequestConfiguratorExtensions
    {
        /// <summary>
        /// 为该请求配置一个处理程序。
        /// </summary>
        /// <typeparam name="TResponse">响应的消息类型。</typeparam>
        /// <param name="requestConfigurator">请求配置。</param>
        /// <param name="handler">处理程序。</param>
        /// <exception cref="ArgumentNullException"><paramref name="requestConfigurator"/> 或 <paramref name="handler"/> 为 null。</exception>
        public static void Handle<TResponse>(this RequestConfigurator requestConfigurator, Action<TResponse> handler)
        {
            if (requestConfigurator == null)
                throw new ArgumentNullException("requestConfigurator");
            if (handler == null)
                throw new ArgumentNullException("handler");

            requestConfigurator.Handle(responseMessage =>
            {
                var result = responseMessage.Result;
                //如果响应的结果类型不是需要捕获的类型则跳过。
                if (result is TResponse)
                    handler((TResponse)result);
            });
        }

        /// <summary>
        /// 为该请求配置一个处理程序。
        /// </summary>
        /// <param name="requestConfigurator">请求配置。</param>
        /// <param name="handler">处理程序。</param>
        /// <exception cref="ArgumentNullException"><paramref name="requestConfigurator"/> 或 <paramref name="handler"/> 为 null。</exception>
        public static void Handle(this RequestConfigurator requestConfigurator, Action handler)
        {
            if (requestConfigurator == null)
                throw new ArgumentNullException("requestConfigurator");
            if (handler == null)
                throw new ArgumentNullException("handler");

            requestConfigurator.Handle(responseMessage => handler());
        }
    }
}