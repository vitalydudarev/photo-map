using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PhotoMap.Api.Services.Interfaces;
using PhotoMap.Api.Settings;

namespace PhotoMap.Api.Services.Implementations
{
    public class StorageServiceClient : IStorageService
    {
        private readonly HttpClient _httpClient;
        private readonly StorageServiceSettings _settings;

        public StorageServiceClient(IHttpClientFactory clientFactory, IOptions<StorageServiceSettings> options)
        {
            _httpClient = clientFactory.CreateClient("storageClient");
            _settings = options.Value;
        }

        public async Task<byte[]> GetFileAsync(long fileId)
        {
            var url = _settings.ApiUrl + "/" + _settings.GetFileEndpoint + fileId;
            var responseMessage = await _httpClient.GetAsync(url);
            var bytes = await responseMessage.Content.ReadAsByteArrayAsync();

            return bytes;
        }

        public async Task<FileInfo> GetFileInfoAsync(long fileId)
        {
            var url = _settings.ApiUrl + "/" + _settings.GetFileEndpoint + fileId + "/info";
            var responseMessage = await _httpClient.GetAsync(url);
            var serialized = await responseMessage.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FileInfo>(serialized);
        }

        public async Task DeleteFileAsync(long fileId)
        {
            var url = _settings.ApiUrl + "/" + fileId;
            var responseMessage = await _httpClient.DeleteAsync(url);
        }

        public async Task DeleteAllFilesAsync()
        {
            var url = _settings.ApiUrl + "/" + _settings.DeleteAllFilesEndpoint;
            var responseMessage = await _httpClient.DeleteAsync(url);
            var deserialized = await responseMessage.Content.ReadAsByteArrayAsync();
        }
    }
}
