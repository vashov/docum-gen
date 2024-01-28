using DocumGen.Application.Contracts.MessageBus;
using DocumGen.Application.Contracts.MessageBus.Messages;
using DocumGen.Application.Services.FileOrders;
using DocumGen.Application.Services.FileOrders.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DocumGen.Api.Workers
{
    public class FileConvertedWorker : IHostedService
    {
        private readonly IMessageConsumer _messageConsumer;
        private readonly IMessageBusConfiguration _messageBusConfiguration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FileConvertedWorker> _logger;

        private string WorkerName => nameof(FileConvertedWorker);

        public FileConvertedWorker(
            IMessageConsumer messageConsumer,
            IMessageBusConfiguration messageBusConfiguration,
            IServiceProvider serviceProvider,
            ILogger<FileConvertedWorker> logger)
        {
            _messageConsumer = messageConsumer;
            _messageBusConfiguration = messageBusConfiguration;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellation)
        {
            _logger.LogInformation("{WorkerName} started at {Date}", WorkerName, DateTimeOffset.UtcNow);

            _messageConsumer.StartConsuming<FileOrderMessage>(_messageBusConfiguration.QueueFileOrderProcessed, HandleFileOrderProcessed);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellation)
        {
            _logger.LogInformation("{WorkerName} stopped at {Date}", WorkerName, DateTimeOffset.UtcNow);
            return Task.CompletedTask;
        }

        private async Task HandleFileOrderProcessed(FileOrderMessage fileOrderMessage)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            var fileOrderService = scope.ServiceProvider.GetService<IFileOrderService>();

            _logger.LogInformation("{WorkerName} start handling {FileOrderId} at {Date}",
                WorkerName, fileOrderMessage.FileOrderId, DateTimeOffset.UtcNow);

            var updateStatusRequest = new FileOrderUpdateStatusRequest
            {
                FileOrderId = fileOrderMessage.FileOrderId,
                Status = fileOrderMessage.Status
            };

            bool statusUpdated = await fileOrderService.UpdateStatus(updateStatusRequest);
            if (statusUpdated)
            {
                _logger.LogInformation("{WorkerName} finish handling {FileOrderId} at {Date}",
                    WorkerName, fileOrderMessage.FileOrderId, DateTimeOffset.UtcNow);
            }
            else
            {
                _logger.LogWarning("{WorkerName} fail handling {FileOrderId} at {Date}",
                    WorkerName, fileOrderMessage.FileOrderId, DateTimeOffset.UtcNow);

                // TODO: Add custom exception.
                throw new InvalidOperationException();
            }
        }
    }
}
