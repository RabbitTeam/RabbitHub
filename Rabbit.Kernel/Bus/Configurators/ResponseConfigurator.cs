using System;

namespace Rabbit.Kernel.Bus.Configurators
{
    /// <summary>
    /// 响应配置。
    /// </summary>
    public class ResponseConfigurator
    {
        private readonly IBus _bus;
        private readonly MessageBase _message;

        /// <summary>
        /// 初始化一个新的响应配置。
        /// </summary>
        /// <param name="bus">总线。</param>
        /// <param name="message">请求消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bus"/> 或 <paramref name="message"/> 为 null。</exception>
        public ResponseConfigurator(IBus bus, MessageBase message)
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (message == null)
                throw new ArgumentNullException("message");

            _bus = bus;
            _message = message;
        }

        /// <summary>
        /// 提交一个响应给请求消息。
        /// </summary>
        /// <param name="configuration">响应消息配置。</param>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> 为null。</exception>
        public void Post(Action<ResponseMessage> configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            var message = new ResponseMessage(_message.Id);
            configuration(message);
            _bus.Publish(message);
        }
    }

    /// <summary>
    /// 响应配置扩展。
    /// </summary>
    public static class ResponseConfiguratorExtensions
    {
        /// <summary>
        /// 提交一个结果给请求消息。
        /// </summary>
        /// <param name="responseConfigurator">响应配置。</param>
        /// <param name="result">结果。</param>
        /// <exception cref="ArgumentNullException"><paramref name="responseConfigurator"/> 为null。</exception>
        public static void Post(this ResponseConfigurator responseConfigurator, object result)
        {
            if (responseConfigurator == null)
                throw new ArgumentNullException("responseConfigurator");
            responseConfigurator.Post(message => message.Result = result);
        }

        /// <summary>
        /// 提交一个结果给请求消息。
        /// </summary>
        /// <param name="responseConfigurator">响应配置。</param>
        /// <param name="result">结果。</param>
        /// <param name="exception">响应异常。</param>
        /// <exception cref="ArgumentNullException"><paramref name="responseConfigurator"/> 为null。</exception>
        public static void Post(this ResponseConfigurator responseConfigurator, object result, Exception exception)
        {
            if (responseConfigurator == null)
                throw new ArgumentNullException("responseConfigurator");
            responseConfigurator.Post(message =>
            {
                message.Result = result;
                message.Exception = exception;
            });
        }
    }
}