using DocumGen.Application.Contracts.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;

namespace DocumGen.MessageBus.RabbitMq
{
    public static class ServiceRegistration
    {
        public static IServiceCollection TryAddRabbitPublisher(this IServiceCollection services)
        {
            services.TryAddRabbitConection();
            services.TryAddSingleton<IMessagePublisher, RabbitMessagePublisher>();

            return services;
        }

        public static IServiceCollection TryAddRabbitConsumer(this IServiceCollection services)
        {
            services.TryAddRabbitConection();
            services.TryAddSingleton<IMessageConsumer, RabbitMessageConsumer>();

            return services;
        }

        private static IServiceCollection TryAddRabbitConection(this IServiceCollection services)
        {
            services.TryAddSingleton<IMessageBusConfiguration, RabbitConfiguration>();

            services.TryAddSingleton<IConnection>(serviceProvider =>
            {
                var messageBusConfiguration = serviceProvider.GetService<IMessageBusConfiguration>();

                var factory = new ConnectionFactory()
                {
                    HostName = messageBusConfiguration.HostName,
                    Port = messageBusConfiguration.Port,
                    UserName = messageBusConfiguration.UserName,
                    Password = messageBusConfiguration.Password
                };
                return factory.CreateConnection();
            });

            services.TryAddSingleton<IModel>(serviceProvider =>
            {
                var rabbitConnection = serviceProvider.GetService<IConnection>();
                IModel channel = rabbitConnection.CreateModel();

                var messageBusConfiguration = serviceProvider.GetService<IMessageBusConfiguration>();

                channel.QueueDeclare(messageBusConfiguration.QueueFileOrderNew, durable: true, exclusive: false, autoDelete: false, null);
                channel.QueueDeclare(messageBusConfiguration.QueueFileOrderProcessed, durable: true, exclusive: false, autoDelete: false, null);

                return channel;
            });

            return services;
        }
    }
}