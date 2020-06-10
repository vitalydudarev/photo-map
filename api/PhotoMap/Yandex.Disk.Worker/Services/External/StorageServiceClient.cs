using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Yandex.Disk.Worker.Services.External
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

        public async Task UploadFileAsync(string fileName, byte[] fileContents)
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

                            var response = await _httpClient.PostAsync(_url, form);
                            if (response.StatusCode != HttpStatusCode.Created)
                                throw new Exception($"Error during uploading {fileName}.");
                        }
                    }
                }
            }
        }
    }
}
