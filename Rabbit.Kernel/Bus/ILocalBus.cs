namespace Rabbit.Kernel.Bus
{
    /// <summary>
    /// 一个抽象的本地总线，该总线所传递的消息只在当前应用程序中有效。
    /// </summary>
    public interface ILocalBus : IBus, ISingletonDependency
    {
    }
}