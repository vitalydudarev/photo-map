using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Storage.Service.Storage
{
    public class FileStorage : IFileStorage
    {
        private readonly string _baseDirectory;

        public FileStorage(IOptions<FileStorageSettings> options)
        {
            _baseDirectory = options.Value.BasePath;
        }

        public Task<byte[]> GetAsync(string fileName)
        {
            string filePath = GetFilePath(fileName);

            return File.ReadAllBytesAsync(filePath);
        }

        public async Task<string> SaveAsync(string fileName, byte[] bytes)
        {
            string filePath = GetFilePath(fileName);

            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await File.WriteAllBytesAsync(filePath, bytes);

            return filePath;
        }

        public void Delete(string fileName)
        {
            string filePath = GetFilePath(fileName);

            File.Delete(filePath);
        }

        private string GetFilePath(string fileName) => Path.Combine(_baseDirectory, fileName);
    }
}
