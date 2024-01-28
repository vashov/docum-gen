using DocumGen.Application.Contracts.MessageBus;
using DocumGen.Application.Contracts.Pagination;
using DocumGen.Application.Services.FileOrders;
using DocumGen.Application.Services.FileOrders.Models;
using DocumGen.Domain.Entities;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DocumGen.Api.Services
{
    public class HealthCheck : IHealthCheck
    {
        private readonly IFileOrderService _fileOrderService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageConsumer _messageConsumer;

        public HealthCheck(
            IFileOrderService fileOrderService,
            IMessagePublisher messagePublisher,
            IMessageConsumer messageConsumer
            ) 
        {
           _fileOrderService = fileOrderService;
           _messagePublisher = messagePublisher;
           _messageConsumer = messageConsumer;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await CheckFileOrderService();
                return HealthCheckResult.Healthy("Ok");
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, ex.Message);
            }
        }

        private async Task CheckFileOrderService()
        {
            var listRequest = new FileOrderGetListRequest
            {
                PageNumber = 1,
                PageSize = 1
            };
            PageResult<FileOrder> _ = await _fileOrderService.GetList(listRequest);
        }
    }
}
