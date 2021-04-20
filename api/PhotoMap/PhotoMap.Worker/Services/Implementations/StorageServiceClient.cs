using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PhotoMap.Worker.Services.Definitions;
using PhotoMap.Worker.Services.DTOs;
using PhotoMap.Worker.Settings;

namespace PhotoMap.Worker.Services.Implementations
{
    public class StorageServiceClient : IStorageService
    {
        private readonly string _url;
        private readonly HttpClient _httpClient;

        public StorageServiceClient(
            IHttpClientFactory clientFactory,
            IOptions<StorageServiceSettings> storageServiceOptions)
        {
            _httpClient = clientFactory.CreateClient("storageClient");
            _url = storageServiceOptions.Value.ApiUrl;
        }

        public async Task<byte[]> GetFileAsync(long fileId)
        {
            var url = _url + "/" + fileId;
            var responseMessage = await _httpClient.GetAsync(url);
            var deserialized = await responseMessage.Content.ReadAsByteArrayAsync();

            return deserialized;
        }

        public async Task<StorageServiceFileDto> SaveFileAsync(string fileName, byte[] fileContents)
        {
            using (var form = new MultipartFormDataContent())
            {
                using (var memoryStream = new MemoryStream(fileContents))
                {
                    using (var streamContent = new StreamContent(memoryStream))
                    {
                        // TODO: remove ReadAsByteArrayAsync()
                        var content = await streamContent.ReadAsByteArrayAsync();

                        using (var fileContent = new ByteArrayContent(content))
                        {
                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                            form.Add(fileContent, "file", fileName);

                            var response = await _httpClient.PostAsync(_url, form);
                            if (response.StatusCode != HttpStatusCode.Created)
                                throw new Exception($"Error during uploading {fileName}.");

                            var responseContent = await response.Content.ReadAsStringAsync();
                            var deserialized = JsonConvert.DeserializeObject<StorageServiceFileDto>(responseContent);

                            return deserialized;
                        }
                    }
                }
            }
        }

        public async Task DeleteFileAsync(long fileId)
        {
            var url = _url + "/" + fileId;
            var responseMessage = await _httpClient.DeleteAsync(url);
        }
    }
}
