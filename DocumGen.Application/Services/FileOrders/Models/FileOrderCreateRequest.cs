using System.IO;

namespace DocumGen.Application.Services.FileOrders.Models
{
    public class FileOrderCreateRequest
    {
        public string FileName { get; set; }
        public Stream FileStream { get; set; }
    }
}
