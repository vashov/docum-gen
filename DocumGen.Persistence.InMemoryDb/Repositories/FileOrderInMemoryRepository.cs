using DocumGen.Application.Contracts.Pagination;
using DocumGen.Application.Contracts.Persistence;
using DocumGen.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumGen.Persistence.InMemoryDb.Repositories
{
    /// <summary>
    /// InMemory Repository for local testing.
    /// </summary>
    public class FileOrderInMemoryRepository : IFileOrderRepository
    {
        private readonly List<FileOrder> _fileOrders = new();

        public Task<FileOrder> AddAsync(FileOrder entity)
        {
            entity.FileOrderId = Guid.NewGuid();
            entity.Status = FileOrderStatus.Created;
            _fileOrders.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var item = _fileOrders.FirstOrDefault(x => x.FileOrderId == id);
            if (item == null)
                return Task.FromResult(false);

            _fileOrders.Remove(item);
            return Task.FromResult(true);
        }

        public Task<FileOrder> GetByIdAsync(Guid id)
        {
            var item = _fileOrders.FirstOrDefault(x => x.FileOrderId == id);
            return Task.FromResult(item);
        }

        public Task<PageResult<FileOrder>> ListAsync(PageQuery pageQuery)
        {
            var totalItems = _fileOrders.Count;
            var items = _fileOrders
                .Skip( (pageQuery.PageNumber - 1) * pageQuery.PageSize)
                .Take(pageQuery.PageSize)
                .ToList();

            var page = new PageResult<FileOrder>
            {
                PageNumber = pageQuery.PageNumber,
                PageSize = pageQuery.PageSize,
                TotalItems = totalItems,
                Data = items
            };
            return Task.FromResult(page);
        }

        public Task<bool> UpdateAsync(FileOrder entity)
        {
            var item = _fileOrders.FirstOrDefault(x => x.FileOrderId == entity.FileOrderId);
            if (item == null)
                return Task.FromResult(false);

            item.UpdatedAt = DateTimeOffset.UtcNow;
            item.Status = entity.Status;
            return Task.FromResult(true);
        }
    }
}
