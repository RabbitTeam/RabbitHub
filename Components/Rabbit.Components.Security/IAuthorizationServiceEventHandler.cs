using Rabbit.Kernel.Events;

namespace Rabbit.Components.Security
{
    /// <summary>
    /// 一个抽象的授权服务事件处理程序。
    /// </summary>
    public interface IAuthorizationServiceEventHandler : IEventHandler
    {
        /// <summary>
        /// 检查前执行。
        /// </summary>
        /// <param name="context"></param>
        void Checking(CheckAccessContext context);

        /// <summary>
        /// 调整完成后执行。
        /// </summary>
        /// <param name="context"></param>
        void Adjust(CheckAccessContext context);

        /// <summary>
        /// 授权完成后执行。
        /// </summary>
        /// <param name="context"></param>
        void Complete(CheckAccessContext context);
    }
}