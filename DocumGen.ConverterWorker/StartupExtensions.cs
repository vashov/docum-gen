using DocumGen.Application;
using DocumGen.Application.Jobs;
using DocumGen.FileStorages;
using DocumGen.MessageBus.RabbitMq;
using Microsoft.Extensions.DependencyInjection;

namespace DocumGen.ConverterWorker
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.TryAddRabbitConsumer();
            services.TryAddRabbitPublisher();

            services.TryAddFileStorages();
            services.TryAddFileConverter();

            services.AddHostedService<FileConversionWorker>();

            return services;
        }
    }
}
