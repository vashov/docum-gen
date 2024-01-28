using DocumGen.Domain.Entities;

namespace DocumGen.Application.Services.FileOrders.Models
{
    public class FileOrderUpdateStatusRequest : FileOrderRequest
    {
        public FileOrderStatus Status { get; set; }
    }
}
