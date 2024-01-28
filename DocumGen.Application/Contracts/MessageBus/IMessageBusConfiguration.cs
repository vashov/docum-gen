namespace DocumGen.Application.Contracts.MessageBus
{
    public interface IMessageBusConfiguration
    {
        string HostName { get; }
        int Port { get; }
        string UserName { get; }
        string Password { get; }

        public string QueueFileOrderNew { get; }
        public string QueueFileOrderProcessed { get; }
    }
}
