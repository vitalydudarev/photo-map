using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Users;
using Microsoft.Extensions.Logging;
using PhotoMap.Worker.Models;
using PhotoMap.Worker.Services.Definitions;

namespace PhotoMap.Worker.Services.Implementations
{
    public class DropboxDownloadService
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private DropboxClient _dropboxClient;
        private readonly IStorageService _storageService;
        private readonly ILogger<DropboxDownloadService> _logger;
        private const string CameraUploads = "/Camera Uploads";

        public DropboxDownloadService(IStorageService storageService, ILogger<DropboxDownloadService> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        public async IAsyncEnumerable<DropboxFile> DownloadAsync(
            string apiToken,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            CreateClient(apiToken);

            var dropboxClientAccount = await _dropboxClient.Users.GetCurrentAccountAsync();
            var listFolderResult = await _dropboxClient.Files.ListFolderAsync(CameraUploads);

            await foreach (var dropboxFile in DownloadFiles(listFolderResult, dropboxClientAccount))
            {
                yield return dropboxFile;
            }

            while (listFolderResult.HasMore)
            {
                listFolderResult = await _dropboxClient.Files.ListFolderContinueAsync(listFolderResult.Cursor);

                await foreach (var dropboxFile in DownloadFiles(listFolderResult, dropboxClientAccount))
                {
                    yield return dropboxFile;
                }
            }

            yield return null;
        }

        private async IAsyncEnumerable<DropboxFile> DownloadFiles(ListFolderResult listFolderResult,
            FullAccount dropboxClientAccount)
        {
            foreach (var entry in listFolderResult.Entries)
            {
                var dropboxFile = await DownloadFileAsync(entry, dropboxClientAccount);
                if (dropboxFile == null)
                    yield break;

                yield return dropboxFile;
            }
        }

        private async Task<DropboxFile> DownloadFileAsync(Metadata metadata, FullAccount dropboxClientAccount)
        {
            var metadataName = metadata.Name;

            try
            {
                _logger.LogInformation($"Dropbox: started downloading {metadataName}.");

                var path = CameraUploads + "/" + metadataName;
                var fileMetadata = await _dropboxClient.Files.DownloadAsync(path);
                var fileContents = await fileMetadata.GetContentAsByteArrayAsync();

                _logger.LogInformation($"Dropbox: finished downloading {metadataName}.");
                _logger.LogInformation($"Dropbox: started saving {metadataName}.");

                var filePath = Path.Combine("Dropbox", dropboxClientAccount.Email, metadataName);
                var createdOn = fileMetadata.Response.ClientModified;

                var savedFileDto = _storageService.SaveFileAsync(filePath, fileContents);

                var dropboxFile = new DropboxFile(dropboxClientAccount.Email, dropboxClientAccount.AccountId,
                    metadataName, filePath, savedFileDto.Id, path, createdOn);

                _logger.LogInformation($"Dropbox: finished saving {metadataName}.");

                return dropboxFile;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed downloading/saving {metadataName}: {e.Message}.");
            }

            return null;
        }

        private void CreateClient(string apiToken)
        {
            var config = new DropboxClientConfig("PhotoMap") { HttpClient = HttpClient };

            _dropboxClient = new DropboxClient(apiToken, config);
        }
    }
}
