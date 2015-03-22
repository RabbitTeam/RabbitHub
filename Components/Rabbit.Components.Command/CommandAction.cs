namespace Rabbit.Components.Command
{
    /// <summary>
    /// 命令的动作。
    /// </summary>
    public enum CommandAction
    {
        /// <summary>
        /// 空。
        /// </summary>
        None,

        /// <summary>
        /// 获取。
        /// </summary>
        Get,

        /// <summary>
        /// 添加。
        /// </summary>
        Add,

        /// <summary>
        /// 发送。
        /// </summary>
        Send,

        /// <summary>
        /// 删除。
        /// </summary>
        Remove,

        /// <summary>
        /// 删除。
        /// </summary>
        Delete,

        /// <summary>
        /// 设置。
        /// </summary>
        Set,

        /// <summary>
        /// 更新。
        /// </summary>
        Update
    }
}