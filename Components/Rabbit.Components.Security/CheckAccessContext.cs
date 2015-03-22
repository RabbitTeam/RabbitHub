using Rabbit.Components.Security.Permissions;

namespace Rabbit.Components.Security
{
    /// <summary>
    /// 检查访问许可上下文。
    /// </summary>
    public class CheckAccessContext
    {
        /// <summary>
        /// 许可模型。
        /// </summary>
        public Permission Permission { get; set; }

        /// <summary>
        /// 需要被检查的用户。
        /// </summary>
        public IUser User { get; set; }

        /// <summary>
        /// 是否通过许可认证。
        /// </summary>
        public bool Granted { get; set; }

        /// <summary>
        /// 允许外部事件进行权限调整，如果为true框架则会再次进行权限检查，上限为3次。
        /// </summary>
        public bool Adjusted { get; set; }
    }
}