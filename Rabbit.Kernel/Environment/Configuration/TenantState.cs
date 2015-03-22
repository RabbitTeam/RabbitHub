namespace Rabbit.Kernel.Environment.Configuration
{
    /// <summary>
    /// 租户状态。
    /// </summary>
    public enum TenantState
    {
        /// <summary>
        /// 未初始化。
        /// </summary>
        Uninitialized,

        /// <summary>
        /// 正在运行。
        /// </summary>
        Running,

        /// <summary>
        /// 关闭。
        /// </summary>
        Disabled,

        /// <summary>
        /// 无效。
        /// </summary>
        Invalid
    }
}