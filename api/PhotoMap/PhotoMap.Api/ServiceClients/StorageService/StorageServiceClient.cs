using System.Net.Http;
using System.Text;
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
            var responseStream = await responseMessage.Content.ReadAsStreamAsync();

            var deserialized = await System.Text.Json.JsonSerializer.DeserializeAsync<byte[]>(responseStream);
            
            return deserialized;
        }
    }
}