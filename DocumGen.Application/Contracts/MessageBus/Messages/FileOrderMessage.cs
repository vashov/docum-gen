using DocumGen.Domain.Entities;
using System;

namespace DocumGen.Application.Contracts.MessageBus.Messages
{
    public class FileOrderMessage
    {
        public Guid FileOrderId { get; set; }
        public FileOrderStatus Status { get; set; }
        public string FileNameSource { get; set; }
        public string FileNameResult { get; set; }
    }
}
