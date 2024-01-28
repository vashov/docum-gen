using DocumGen.Application.Contracts.MessageBus;
using DocumGen.Application.Contracts.MessageBus.Messages;
using DocumGen.Application.Services.FileConverters;
using DocumGen.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DocumGen.Application.Jobs
{
    public class FileConversionWorker : IHostedService
    {
        private readonly IMessageConsumer _messageConsumer;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageBusConfiguration _messageBusConfiguration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FileConversionWorker> _logger;

        private string WorkerName => nameof(FileConversionWorker);

        public FileConversionWorker(
            IMessageConsumer messageConsumer,
            IMessagePublisher messagePublisher,
            IMessageBusConfiguration messageBusConfiguration,
            IServiceProvider serviceProvider,
            ILogger<FileConversionWorker> logger)
        {
            _messageConsumer = messageConsumer;
            _messagePublisher = messagePublisher;
            _messageBusConfiguration = messageBusConfiguration;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellation)
        {
            _logger.LogInformation("{WorkerName} started at {Date}", WorkerName, DateTimeOffset.UtcNow);

            await PrepareBrowser(cancellation);

            _messageConsumer.StartConsuming<FileOrderMessage>(_messageBusConfiguration.QueueFileOrderNew, HandleFileOrderNew);
        }

        public Task StopAsync(CancellationToken cancellation)
        {
            _logger.LogInformation("{WorkerName} stopped at {Date}", WorkerName, DateTimeOffset.UtcNow);
            return Task.CompletedTask;
        }

        private async Task PrepareBrowser(CancellationToken cancellation)
        {
            _logger.LogInformation("{WorkerName} prepare browser at {Date}", WorkerName, DateTimeOffset.UtcNow);

            using IServiceScope scope = _serviceProvider.CreateScope();
            var fileConverter = scope.ServiceProvider.GetService<IFileConverter>();

            await fileConverter.PrepareBrowser(cancellation);
        }

        private async Task HandleFileOrderNew(FileOrderMessage fileOrderMessage)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            var fileConverter = scope.ServiceProvider.GetService<IFileConverter>();

            _logger.LogInformation("{WorkerName} start process {FileOrderId} at {Date}",
                WorkerName, fileOrderMessage.FileOrderId, DateTimeOffset.UtcNow);

            // TODO: Use cancelation token.
            bool isConverted = await fileConverter.ConvertHtmlToPdf(
                fileOrderMessage.FileNameSource, fileOrderMessage.FileNameResult, cancellation: default);

            if (isConverted)
            {
                // TODO: FileOrder can be deleted before we finished Converting.

                fileOrderMessage.Status = FileOrderStatus.Processed;
                _messagePublisher.PublishMessage(_messageBusConfiguration.QueueFileOrderProcessed, fileOrderMessage);

                _logger.LogInformation("{WorkerName} finish process {FileOrderId} at {Date}",
                    WorkerName, fileOrderMessage.FileOrderId, DateTimeOffset.UtcNow);
            }
            else
            {
                _logger.LogWarning("{WorkerName} fail process {FileOrderId} at {Date}",
                    WorkerName, fileOrderMessage.FileOrderId, DateTimeOffset.UtcNow);

                // TODO: Add custom exception.
                throw new InvalidOperationException();
            }
        }
    }
}
