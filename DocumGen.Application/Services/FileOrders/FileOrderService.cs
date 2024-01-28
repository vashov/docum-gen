using DocumGen.Application.Contracts.FileStorages;
using DocumGen.Application.Contracts.MessageBus;
using DocumGen.Application.Contracts.MessageBus.Messages;
using DocumGen.Application.Contracts.Pagination;
using DocumGen.Application.Contracts.Persistence;
using DocumGen.Application.Services.FileOrders.Models;
using DocumGen.Application.Services.FileOrders.Validation;
using DocumGen.Domain.Entities;
using FluentValidation;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DocumGen.Application.Services.FileOrders
{
    public class FileOrderService : IFileOrderService
    {
        private readonly IFileOrderRepository _fileOrderRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageBusConfiguration _messageBusConfiguration;

        public FileOrderService(
            IFileOrderRepository fileOrderRepository,
            IFileStorage fileStorage,
            IMessagePublisher messagePublisher,
            IMessageBusConfiguration messageBusConfiguration
            )
        {
            _fileOrderRepository = fileOrderRepository;
            _fileStorage = fileStorage;
            _messagePublisher = messagePublisher;
            _messageBusConfiguration = messageBusConfiguration;
        }

        public async Task<FileOrder> Create(FileOrderCreateRequest request)
        {
            new FileOrderCreateRequestValidator().ValidateAndThrow(request);

            var createdAt = DateTimeOffset.UtcNow;
            string fileNameSource = PrepareFileName(request.FileName, createdAt);
            string fileNameResult = BuildPdfFileName(fileNameSource);

            var fileOrderEntity = new FileOrder
            {
                FileNameSource = fileNameSource,
                FileNameResult = fileNameResult,
                CreatedAt = createdAt,
                Status = FileOrderStatus.Created
            };

            FileOrder fileOrderCreated = await _fileOrderRepository.AddAsync(fileOrderEntity);

            bool isFileSaved = await _fileStorage.CreateWrite(fileNameSource, request.FileStream);
            if (!isFileSaved)
            {
                fileOrderCreated.Status = FileOrderStatus.Failed;
                await _fileOrderRepository.UpdateAsync(fileOrderCreated);
                return null;
            }

            var fileOrderMessage = new FileOrderMessage
            {
                FileOrderId = fileOrderCreated.FileOrderId,
                Status = fileOrderCreated.Status,
                FileNameSource = fileOrderCreated.FileNameSource,
                FileNameResult = fileOrderCreated.FileNameResult
            };
            _messagePublisher.PublishMessage(_messageBusConfiguration.QueueFileOrderNew, fileOrderMessage);

            return fileOrderCreated;
        }

        public async Task<FileOrder> Get(FileOrderRequest request)
        {
            new FileOrderRequestValidator().ValidateAndThrow(request);

            var fileOrder = await _fileOrderRepository.GetByIdAsync(request.FileOrderId);

            return fileOrder;
        }

        public async Task<bool> DeleteFiles(FileOrderRequest request)
        {
            new FileOrderRequestValidator().ValidateAndThrow(request);

            var fileOrder = await _fileOrderRepository.GetByIdAsync(request.FileOrderId);

            Task deleteFileSource = _fileStorage.DeleteIfExist(fileOrder.FileNameSource);
            Task deleteFileResult = _fileStorage.DeleteIfExist(fileOrder.FileNameResult);

            await Task.WhenAll(deleteFileSource, deleteFileResult);

            fileOrder.Status = FileOrderStatus.FilesDeleted;

            bool isUpdated = await _fileOrderRepository.UpdateAsync(fileOrder);

            return isUpdated;
        }

        public async Task<bool> UpdateStatus(FileOrderUpdateStatusRequest request)
        {
            new FileOrderRequestValidator().ValidateAndThrow(request);

            FileOrder fileOrder = await _fileOrderRepository.GetByIdAsync(request.FileOrderId);
            if (fileOrder == null)
            {
                return false;
            }
            fileOrder.Status = request.Status;

            bool isUpdated = await _fileOrderRepository.UpdateAsync(fileOrder);
            return isUpdated;
        }

        public async Task<Stream> GetFileResult(FileOrderRequest request)
        {
            new FileOrderRequestValidator().ValidateAndThrow(request);

            FileOrder fileOrder = await _fileOrderRepository.GetByIdAsync(request.FileOrderId);

            if (fileOrder.Status != FileOrderStatus.Processed)
            {
                // TODO: validation error
                return null;
            }

            return _fileStorage.OpenRead(fileOrder.FileNameResult);
        }

        public async Task<PageResult<FileOrder>> GetList(FileOrderGetListRequest request)
        {
            new FileOrderGetListRequestValidator().ValidateAndThrow(request);

            PageResult<FileOrder> page = await _fileOrderRepository.ListAsync(request);

            return page;
        }

        private static string PrepareFileName(string fileName, DateTimeOffset createdAt)
        {
            string firstPartFileName = Path.GetFileNameWithoutExtension(fileName);
            string fileNameExtension = Path.GetExtension(fileName);
            firstPartFileName = firstPartFileName.Replace(".", "_");

            // 2024-12-30T23:50:40.1234567 -> 20241230_235040_1234567
            string createdAtFormatted = createdAt.ToString("O").Replace("-", "").Replace("T", "_").Replace(":", "").Replace(".", "_");
            int startIdxTimezone = createdAtFormatted.IndexOf("+");
            createdAtFormatted = createdAtFormatted.Substring(0, startIdxTimezone);

            return $"{firstPartFileName}_{createdAtFormatted}{fileNameExtension}";
        }

        private static string BuildPdfFileName(string fileNameSource)
        {
            string[] sourceFileNameParts = fileNameSource.Split('.');
            string clearFileName = sourceFileNameParts[0];
            string pdfFileName = clearFileName + ".pdf";
            return pdfFileName;
        }
    }
}
