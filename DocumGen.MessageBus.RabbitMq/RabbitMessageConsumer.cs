using DocumGen.Application.Contracts.MessageBus;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DocumGen.MessageBus.RabbitMq
{
    public class RabbitMessageConsumer : IMessageConsumer, IDisposable
    {
        private readonly IModel _channel;
        private readonly ILogger<RabbitMessageConsumer> _logger;
        private readonly EventingBasicConsumer _consumer;
        private bool _isConsumed;

        public RabbitMessageConsumer(IModel channel, ILogger<RabbitMessageConsumer> logger)
        {
            _channel = channel;
            _logger = logger;
            _consumer = new EventingBasicConsumer(channel);
        }

        public void StartConsuming<T>(string queueName, Func<T, Task> handleMessage)
        {
            _consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                try
                {
                    T typedMessage = JsonSerializer.Deserialize<T>(body, JsonSerializerHelper.GetDefault());

                    await handleMessage(typedMessage);

                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    string message = Encoding.UTF8.GetString(body);
                    _logger.LogError(ex, $"{nameof(RabbitMessageConsumer)} message handling failed: {message}");

                    await Task.Delay(1000);

                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queueName, autoAck: false, _consumer);

            _isConsumed = true;
        }

        public void StopConsuming()
        {
            if (_isConsumed)
            {
                _consumer.Received -= (model, ea) => { };
                _isConsumed = false;
            }
        }

        public void Dispose()
        {
            StopConsuming();
        }
    }
}
