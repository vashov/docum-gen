using DocumGen.Domain.Common;
using System;

namespace DocumGen.Domain.Entities
{
    public class FileOrder : AuditableEntity
    {
        public Guid FileOrderId { get; set; }
        public string FileNameSource { get; set; }
        public string FileNameResult { get; set; }
        public FileOrderStatus Status { get; set; }
    }
}
