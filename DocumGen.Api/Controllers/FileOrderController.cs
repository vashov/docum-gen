using DocumGen.Application.Contracts.Pagination;
using DocumGen.Application.Services.FileOrders;
using DocumGen.Application.Services.FileOrders.Models;
using DocumGen.Domain.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DocumGen.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileOrderController : ControllerBase
    {
        private readonly ILogger<FileOrderController> _logger;
        private readonly IFileOrderService _fileOrderService;

        public FileOrderController(
            ILogger<FileOrderController> logger,
            IFileOrderService fileOrderService)
        {
            _logger = logger;
            _fileOrderService = fileOrderService;
        }

        [HttpGet("[action]")]
        public async Task<PageResult<FileOrder>> List([FromQuery] FileOrderGetListRequest request)
        {
            PageResult<FileOrder> page = await _fileOrderService.GetList(request);

            return page;
        }

        [HttpPost("[action]")]
        public async Task<FileOrder> Create(IFormFile file)
        {
            FileOrder fileOrder;
            using (Stream fileStream = file.OpenReadStream())
            {
                var request = new FileOrderCreateRequest
                {
                    FileName = file.FileName,
                    FileStream = fileStream,
                };
                fileOrder = await _fileOrderService.Create(request);
            }

            return fileOrder;
        }

        [EnableCors("FilePolicy")]
        [HttpGet("[action]")]
        public async Task DownloadFileResult([FromQuery] FileOrderRequest request)
        {
            FileOrder fileOrder = await _fileOrderService.Get(request);
            Stream fileStream = await _fileOrderService.GetFileResult(request);

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            if (!contentTypeProvider.TryGetContentType(fileOrder.FileNameResult, out string contentType))
            {
                contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            Response.Headers.Add(HeaderNames.ContentDisposition, $"attachment; filename=\"{fileOrder.FileNameResult}\"");
            Response.Headers.Add(HeaderNames.ContentType, contentType);

            const int bufferSize = 1024;
            var buffer = new byte[bufferSize];
            while (true)
            {
                var bytesRead = await fileStream.ReadAsync(buffer, 0, bufferSize);
                if (bytesRead == 0)
                    break;
                await Response.Body.WriteAsync(buffer, 0, bytesRead);
            }
            await Response.Body.FlushAsync();
        }

        [HttpPost("[action]")]
        public async Task DeleteFiles([FromBody] FileOrderRequest request)
        {
            await _fileOrderService.DeleteFiles(request);
        }
    }
}