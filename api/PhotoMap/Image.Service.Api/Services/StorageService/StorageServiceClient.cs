using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Image.Service.Services.DTOs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Image.Service.Services.StorageService
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
            var deserialized = await responseMessage.Content.ReadAsByteArrayAsync();

            return deserialized;
        }

        public async Task DeleteFileAsync(long fileId)
        {
            var url = _settings.ApiUrl + "/" + fileId;
            var responseMessage = await _httpClient.DeleteAsync(url);
        }

        public async Task<StorageServiceFileDto> SaveFileAsync(string fileName, byte[] fileContents)
        {
            using (var form = new MultipartFormDataContent())
            {
                using (var memoryStream = new MemoryStream(fileContents))
                {
                    using (var streamContent = new StreamContent(memoryStream))
                    {
                        var content = await streamContent.ReadAsByteArrayAsync();

                        using (var fileContent = new ByteArrayContent(content))
                        {
                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                            form.Add(fileContent, "file", fileName);

                            var response = await _httpClient.PostAsync(_settings.ApiUrl, form);
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
    }
}
