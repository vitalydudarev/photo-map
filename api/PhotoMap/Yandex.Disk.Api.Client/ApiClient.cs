using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Yandex.Disk.Api.Client.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Yandex.Disk.Api.Client
{
    public class ApiClient
    {
        private const string Url = "https://cloud-api.yandex.net/v1/disk";
        private readonly UrlBuilder _urlBuilder;
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
            { PropertyNamingPolicy = new SnakeCaseNamingPolicy() };

        public ApiClient(string oAuthToken, HttpClient client)
        {
            _httpClient = client;
            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"OAuth {oAuthToken}");
            _urlBuilder = new UrlBuilder(NamingConvention.SnakeCase);
        }

        public async Task<Models.Disk> GetDiskAsync(CancellationToken cancellationToken)
        {
            return await GetAsync<Models.Disk>(Url, cancellationToken);
        }

        public async Task<Resource> GetResourceAsync(string path, CancellationToken cancellationToken, int offset = 0, int limit = 20)
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(path), path },
                { nameof(offset), offset.ToString() },
                { nameof(limit), limit.ToString() }
            };

            var url = _urlBuilder.Build(Url, "resources", parameters);

            return await GetAsync<Resource>(url, cancellationToken);
        }

        public async Task<DownloadUrl> GetDownloadUrlAsync(string path, CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(path), path }
            };

            var url = _urlBuilder.Build(Url, "resources/download", parameters);

            return await GetAsync<DownloadUrl>(url, cancellationToken);
        }

        public async Task<FilesResourceList> GetFlatFilesListAsync(CancellationToken cancellationToken, string mediaType = null, int limit = 20)
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(mediaType), mediaType },
                { nameof(limit), limit.ToString() }
            };

            var url = _urlBuilder.Build(Url, "resources/files", parameters);

            return await GetAsync<FilesResourceList>(url, cancellationToken);
        }

        private async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            var responseMessage = await _httpClient.GetAsync(url, cancellationToken);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync();

            if (responseMessage.StatusCode == HttpStatusCode.OK)
                return await JsonSerializer.DeserializeAsync<T>(responseStream, _jsonSerializerOptions, cancellationToken);

            var error = await JsonSerializer.DeserializeAsync<ApiError>(responseStream, _jsonSerializerOptions, cancellationToken);

            throw new ApiException(error);
        }
    }
}
