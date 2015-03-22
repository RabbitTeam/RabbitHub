using Rabbit.Kernel.Works;

namespace Rabbit.Components.Security
{
    /// <summary>
    /// 工作上下文扩展方法。
    /// </summary>
    public static class WorkContentExtensions
    {
        /// <summary>
        /// 获取当前用户。
        /// </summary>
        /// <param name="workContext">工作上下文。</param>
        /// <returns>用户实例。</returns>
        public static IUser GetCurrentUser(this WorkContext workContext)
        {
            return workContext.GetCurrentUser<IUser>();
        }

        /// <summary>
        /// 获取当前用户。
        /// </summary>
        /// <typeparam name="T">用户模型类型。</typeparam>
        /// <param name="workContext">工作上下文。</param>
        /// <returns>用户实例。</returns>
        public static T GetCurrentUser<T>(this WorkContext workContext) where T : IUser
        {
            return workContext.GetState<T>("CurrentUser");
        }
    }
}