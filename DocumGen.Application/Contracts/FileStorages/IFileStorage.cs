using System.IO;
using System.Threading.Tasks;

namespace DocumGen.Application.Contracts.FileStorages
{
    public interface IFileStorage
    {
        Task<bool> Exists(string fileName);
        Stream OpenRead(string fileName);
        Task<bool> CreateWrite(string fileName, Stream file);
        Task<bool> Delete(string fileName);

        /// <returns>True if file doesnt exist or was deleted. False if file exist but was not deleted.</returns>
        Task<bool> DeleteIfExist(string fileName);
    }
}
