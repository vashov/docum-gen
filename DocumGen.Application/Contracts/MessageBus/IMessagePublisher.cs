namespace DocumGen.Application.Contracts.MessageBus
{
    public interface IMessagePublisher
    {
        void PublishMessage<T>(string queueName, T message);
    }
}
