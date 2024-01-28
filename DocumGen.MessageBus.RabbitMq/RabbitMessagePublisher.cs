using DocumGen.Application.Contracts.MessageBus;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace DocumGen.MessageBus.RabbitMq
{
    public class RabbitMessagePublisher : IMessagePublisher
    {
        private readonly IModel _channel;

        public RabbitMessagePublisher(IModel channel)
        {
            _channel = channel;
        }

        public void PublishMessage<T>(string queueName, T message)
        {
            string messageString = JsonSerializer.Serialize(message, JsonSerializerHelper.GetDefault());
            _channel.BasicPublish("", queueName, null, Encoding.UTF8.GetBytes(messageString));
        }
    }
}
