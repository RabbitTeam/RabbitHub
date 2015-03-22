using Rabbit.Kernel;
using Rabbit.Kernel.Localization;
using System;
using System.Runtime.Serialization;

namespace Rabbit.Components.Security
{
    /// <summary>
    /// Rabbit安全异常。
    /// </summary>
    [Serializable]
    public class RabbitSecurityException : RabbitException
    {
        /// <summary>
        /// 实例化一个新的Rabbit安全异常。
        /// </summary>
        /// <param name="message">错误消息。</param>
        public RabbitSecurityException(LocalizedString message)
            : base(message)
        {
        }

        /// <summary>
        /// 实例化一个新的Rabbit安全异常。
        /// </summary>
        /// <param name="message">错误消息。</param>
        /// <param name="innerException">内部异常。</param>
        public RabbitSecurityException(LocalizedString message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// 实例化一个新的Rabbit安全异常。
        /// </summary>
        /// <param name="info">异常信息。</param>
        /// <param name="context">上下文。</param>
        protected RabbitSecurityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 许可名称。
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// 用户模型。
        /// </summary>
        public IUser User { get; set; }
    }
}