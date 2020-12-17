using System.Threading.Tasks;
using PhotoMap.Api.Services.Interfaces;

namespace PhotoMap.Api.Services.Implementations
{
    public class LocalFileProvider : IFileProvider
    {
        private readonly IStorageService _storageService;

        public LocalFileProvider(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<byte[]> GetFileContents(long fileId)
        {
            return await _storageService.GetFileAsync(fileId);
        }
    }
}
