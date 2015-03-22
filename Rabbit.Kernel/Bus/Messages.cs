using System;

namespace Rabbit.Kernel.Bus
{
    /// <summary>
    /// 消息基类。
    /// </summary>
    [Serializable]
    public class MessageBase
    {
        /// <summary>
        /// 初始化一个新的消息。
        /// </summary>
        public MessageBase()
        {
            Id = Guid.NewGuid().ToString();
            PublishDateTimeUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// 消息的唯一标识。
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 发布的Utc时间。
        /// </summary>
        public DateTime PublishDateTimeUtc { get; internal protected set; }

        #region Overrides of Object

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString()
        {
            return string.Format("Type:{0},Id:{1},PublishDateTimeUtc:{2},PublishDateTime:{3}", GetType().FullName, Id, PublishDateTimeUtc,
                PublishDateTimeUtc.ToLocalTime());
        }

        #endregion Overrides of Object
    }

    /// <summary>
    /// 一个响应消息。
    /// </summary>
    [Serializable]
    public class ResponseMessage : MessageBase
    {
        /// <summary>
        /// 初始化一个新的响应消息。
        /// </summary>
        /// <param name="requestMessageId">请求消息的Id。</param>
        public ResponseMessage(string requestMessageId)
        {
            RequestMessageId = requestMessageId;
        }

        /// <summary>
        /// 初始化一个新的响应消息。
        /// </summary>
        /// <param name="responseMessageId">响应消息Id。</param>
        /// <param name="result">请求结果。</param>
        public ResponseMessage(string responseMessageId, object result)
            : this(responseMessageId)
        {
            Result = result;
        }

        /// <summary>
        /// 响应结果。
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// 请求消息的Id。
        /// </summary>
        public string RequestMessageId { get; private set; }

        /// <summary>
        /// 响应时发生的异常，没有发生异常则为null。
        /// </summary>
        public Exception Exception { get; set; }

        #region Overrides of MessageBase

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString()
        {
            var message = base.ToString();
            return message + "," +
                   string.Format("RequestMessageId:{0},Result:{1},Exception:{2}", RequestMessageId, Result, Exception);
        }

        #endregion Overrides of MessageBase
    }

    /// <summary>
    /// 方法消息。
    /// </summary>
    [Serializable]
    public class MethodMessage : MessageBase
    {
        /// <summary>
        /// 初始化一个新的方法消息。
        /// </summary>
        /// <param name="key">键。</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> 为空。</exception>
        public MethodMessage(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            Key = key;
        }

        /// <summary>
        /// 消息键。
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 方法参数。
        /// </summary>
        public object[] Parameters { get; set; }

        #region Overrides of MessageBase

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString()
        {
            var message = base.ToString();
            return message + "," +
                string.Format("Key:{0},Parameters:{1}", Key, Parameters == null ? "null" : string.Join("|", Parameters));
        }

        #endregion Overrides of MessageBase
    }

    /// <summary>
    /// 命令消息。
    /// </summary>
    [Serializable]
    public class CommandMessage : MessageBase
    {
        /// <summary>
        /// 初始化一个新的命令消息。
        /// </summary>
        /// <param name="commandName">命令名称。</param>
        public CommandMessage(string commandName)
            : this(commandName, null)
        {
        }

        /// <summary>
        /// 初始化一个新的命令消息。
        /// </summary>
        /// <param name="commandName">命令名称。</param>
        /// <param name="parameters">命令参数。</param>
        public CommandMessage(string commandName, object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw new ArgumentNullException("commandName");

            CommandName = commandName;
            Parameters = parameters;
        }

        /// <summary>
        /// 命令名称。
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// 参数。
        /// </summary>
        public object[] Parameters { get; set; }

        #region Overrides of MessageBase

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString()
        {
            var message = base.ToString();
            return message + "," +
                string.Format("CommandName:{0},Parameters:{1}", CommandName, Parameters == null ? "null" : string.Join("|", Parameters));
        }

        #endregion Overrides of MessageBase
    }
}