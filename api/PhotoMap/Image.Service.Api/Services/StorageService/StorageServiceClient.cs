using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Image.Service.Services.StorageService
{
    public class StorageServiceClient : IStorageService
    {
        private readonly HttpClient _httpClient;
        private StorageServiceSettings _settings;

        public StorageServiceClient(IHttpClientFactory clientFactory, IOptions<StorageServiceSettings> options)
        {
            _httpClient = clientFactory.CreateClient("storageClient");
            _settings = options.Value;
        }

        public async Task<byte[]> GetFileAsync(long fileId)
        {
            var url = _settings.ApiUrl + "/" + _settings.GetFileEndpoint + fileId;
            var responseMessage = await _httpClient.GetAsync(url);
            var deserialized = await responseMessage.Content.ReadAsByteArrayAsync();

            return deserialized;
        }
    }
}
