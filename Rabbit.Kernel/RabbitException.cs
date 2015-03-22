using Rabbit.Kernel.Localization;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Rabbit.Kernel
{
    /// <summary>
    /// Rabbit异常类。
    /// </summary>
    [Serializable]
    public class RabbitException : ApplicationException
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的Rabbit异常实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public RabbitException(LocalizedString message)
            : base(message.Text)
        {
            LocalizedMessage = message;
        }

        /// <summary>
        /// 初始化一个新的Rabbit异常实例。
        /// </summary>
        /// <param name="message">异常消息。</param>
        /// <param name="innerException">内部异常。</param>
        public RabbitException(LocalizedString message, Exception innerException)
            : base(message.Text, innerException)
        {
            LocalizedMessage = message;
        }

        /// <summary>
        /// 初始化一个新的Rabbit异常实例。
        /// </summary>
        /// <param name="info">保存序列化对象数据的对象。</param>
        /// <param name="context">有关源或目标的上下文信息。</param>
        protected RabbitException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// 本地化异常消息。
        /// </summary>
        public LocalizedString LocalizedMessage { get; private set; }

        #endregion Property

        #region Overrides of Exception

        /// <summary>
        /// 当在派生类中重写时，用关于异常的信息设置 <see cref="T:System.Runtime.Serialization.SerializationInfo"/>。
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo"/>，它存有有关所引发异常的序列化的对象数据。</param><param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext"/>，它包含有关源或目标的上下文信息。</param><exception cref="T:System.ArgumentNullException"><paramref name="info"/> 参数是空引用（Visual Basic 中为 Nothing）。</exception><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/></PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Text", LocalizedMessage.Text);
            info.AddValue("Scope", LocalizedMessage.Scope);
            info.AddValue("TextHint", LocalizedMessage.TextHint);
            var args = LocalizedMessage.Args;
            var argsStr = args == null || !args.Any() ? "null" : string.Join(",", args);
            info.AddValue("Args", argsStr);
        }

        #endregion Overrides of Exception
    }
}