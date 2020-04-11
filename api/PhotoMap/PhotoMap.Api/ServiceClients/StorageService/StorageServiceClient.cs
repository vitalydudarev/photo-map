using System.Net.Http;
using System.Threading.Tasks;

namespace PhotoMap.Api.ServiceClients.StorageService
{
    public class StorageServiceClient : IStorageService
    {
        private readonly string _url;
        private readonly HttpClient _httpClient;

        public StorageServiceClient(IHttpClientFactory clientFactory, string url)
        {
            _httpClient = clientFactory.CreateClient("storageClient");
            _url = url;
        }
        
        public async Task<byte[]> GetFileAsync(string key)
        {
            var responseMessage = await _httpClient.GetAsync(_url + $"/storage/file/{key}");
            var deserialized = await responseMessage.Content.ReadAsByteArrayAsync();
            
            return deserialized;
        }
    }
}