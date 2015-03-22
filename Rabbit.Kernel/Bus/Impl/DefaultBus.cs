namespace Rabbit.Kernel.Bus.Impl
{
    internal sealed class DefaultBus : Bus, ILocalBus
    {
        public DefaultBus(IMessageDispatcher messageDispatcher)
            : base(messageDispatcher)
        {
        }
    }
}