using DocumGen.Application.Contracts.Pagination;
using DocumGen.Application.Services.FileOrders.Models;
using DocumGen.Domain.Entities;
using FluentValidation;
using System.IO;
using System.Threading.Tasks;

namespace DocumGen.Application.Services.FileOrders
{
    public interface IFileOrderService
    {
        /// <exception cref="ValidationException" />
        Task<FileOrder> Create(FileOrderCreateRequest request);

        /// <exception cref="ValidationException" />
        Task<FileOrder> Get(FileOrderRequest request);

        /// <exception cref="ValidationException" />
        Task<bool> UpdateStatus(FileOrderUpdateStatusRequest request);

        /// <exception cref="ValidationException" />
        Task<bool> DeleteFiles(FileOrderRequest request);

        /// <exception cref="ValidationException" />
        Task<PageResult<FileOrder>> GetList(FileOrderGetListRequest request);

        /// <exception cref="ValidationException" />
        Task<Stream> GetFileResult(FileOrderRequest request);
    }
}
