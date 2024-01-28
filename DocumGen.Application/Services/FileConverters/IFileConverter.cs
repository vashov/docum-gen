using System.Threading;
using System.Threading.Tasks;

namespace DocumGen.Application.Services.FileConverters
{
    public interface IFileConverter
    {
        /// <summary>
        /// Download browser if there is no one installed, and return BrowserExecutablePath
        /// </summary>
        Task<string> PrepareBrowser(CancellationToken cancellation = default);
        Task<bool> ConvertHtmlToPdf(string fileNameSource, string fileNameResult, CancellationToken cancellation = default);
    }
}
