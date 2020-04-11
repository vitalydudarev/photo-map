using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Storage.Service.Storage
{
    public class FileStorage : IStorage
    {
        private readonly string _baseDirectory;

        public FileStorage(IOptions<FileStorageSettings> options)
        {
            _baseDirectory = options.Value.BasePath;
        }

        public Task<byte[]> GetAsync(string fileKey)
        {
            string filePath = GetFilePath(fileKey);

            return File.ReadAllBytesAsync(filePath);
        }

        public async Task SaveAsync(string fileKey, byte[] bytes)
        {
            string filePath = GetFilePath(fileKey);

            await File.WriteAllBytesAsync(filePath, bytes);
        }

        private string GetFilePath(string fileKey) => Path.Combine(_baseDirectory, fileKey);
    }
}