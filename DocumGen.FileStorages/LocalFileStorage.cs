using DocumGen.Application.Contracts.FileStorages;
using System.IO;
using System.Threading.Tasks;

namespace DocumGen.FileStorages
{
    public class LocalFileStorage : IFileStorage
    {
        private LocalStorageConfiguration _localStorageConfiguration;

        public LocalFileStorage(LocalStorageConfiguration localStorageConfiguration)
        {
            _localStorageConfiguration = localStorageConfiguration;
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
            return Task.FromResult(File.Exists(fileName));
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
            return Path.Combine(_localStorageConfiguration.Path, fileName);
        }
    }
}
