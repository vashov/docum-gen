using DocumGen.Application.Contracts.FileStorages;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace DocumGen.FileStorages
{
    public class LocalFileStorage : IFileStorage
    {
        private LocalStorageConfiguration _localStorageConfiguration;
        private readonly ILogger<LocalFileStorage> _logger;

        public LocalFileStorage(
            LocalStorageConfiguration localStorageConfiguration,
            ILogger<LocalFileStorage> logger)
        {
            _localStorageConfiguration = localStorageConfiguration;
            _logger = logger;
        }

        public async Task<bool> CreateWrite(string fileName, Stream dataStream)
        {
            fileName = GetFullFileName(fileName);

            Directory.CreateDirectory(_localStorageConfiguration.Path);
            using (var fileStream = File.Create(fileName))
            {
                dataStream.Seek(0, SeekOrigin.Begin);
                await dataStream.CopyToAsync(fileStream);
            }
            return true;
        }

        public Task<bool> Exists(string fileName)
        {
            fileName = GetFullFileName(fileName);
            bool isExists = File.Exists(fileName);
            if (!isExists)
            {
                _logger.LogInformation("File {FileName} does not exist in local storage", fileName);
            }
            return Task.FromResult(isExists);
        }

        public Stream OpenRead(string fileName)
        {
            fileName = GetFullFileName(fileName);
            return File.OpenRead(fileName);
        }

        public Task<bool> Delete(string fileName)
        {
            fileName = GetFullFileName(fileName);
            bool result = true;
            try
            {
                File.Delete(fileName);
            }
            catch
            {
                result = false;
            }
            return Task.FromResult(result);
        }

        public async Task<bool> DeleteIfExist(string fileName)
        {
            if (!await Exists(fileName))
                return true;

            return await Delete(fileName);
        }

        private string GetFullFileName(string fileName)
        {
            string fullFileName = Path.Combine(_localStorageConfiguration.Path, fileName);
            return fullFileName;
        }
    }
}
