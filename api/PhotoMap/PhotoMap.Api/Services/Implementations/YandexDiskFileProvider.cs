using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PhotoMap.Api.Services.Interfaces;
using Yandex.Disk.Api.Client;

namespace PhotoMap.Api.Services.Implementations
{
    public class YandexDiskFileProvider : IFileProvider
    {
        private readonly IUserService _userService;
        private readonly IStorageService _storageService;
        private readonly UserInfo _userInfo;

        private static readonly HttpClient HttpClient = new HttpClient();
        private const string YandexDiskFolder = "disk:/photomap-storage/";

        public YandexDiskFileProvider(IUserService userService, IStorageService storageService, UserInfo userInfo)
        {
            _userService = userService;
            _storageService = storageService;
            _userInfo = userInfo;
        }

        public async Task<byte[]> GetFileContents(long fileId)
        {
            var fileInfo = await _storageService.GetFileInfoAsync(fileId);

            var user = await _userService.GetAsync(_userInfo.UserId);

            var yandexDiskApiClient = new ApiClient(user.YandexDiskAccessToken, HttpClient);
            var downloadUrl = await yandexDiskApiClient.GetDownloadUrlAsync(YandexDiskFolder + fileInfo.FileName, new CancellationToken());

            return await HttpClient.GetByteArrayAsync(downloadUrl.Href);
        }
    }
}
