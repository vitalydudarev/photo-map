using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Auth;
using Dropbox.Api.Files;
using Dropbox.Api.Users;
using Microsoft.Extensions.Logging;
using PhotoMap.Worker.Models;
using PhotoMap.Worker.Services.Definitions;

namespace PhotoMap.Worker.Services.Implementations
{
    public class DropboxDownloadService : IDisposable
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private DropboxClient _dropboxClient;
        private readonly IStorageService _storageService;
        private readonly IDropboxDownloadStateService _stateService;
        private readonly IDropboxProgressReporter _progressReporter;
        private readonly ILogger<DropboxDownloadService> _logger;
        private DropboxDownloadState _state;
        private int _lastProcessedFileIndex = -1;

        private const string CameraUploads = "/Camera Uploads";
        private const int Limit = 2000;
        private const int ListFolderMaxLimit = 2000;

        public DropboxDownloadService(
            IStorageService storageService,
            IDropboxDownloadStateService stateService,
            IDropboxProgressReporter progressReporter,
            ILogger<DropboxDownloadService> logger)
        {
            _storageService = storageService;
            _stateService = stateService;
            _progressReporter = progressReporter;
            _logger = logger;
        }

        public async IAsyncEnumerable<DropboxFile> DownloadAsync(
            string apiToken,
            StoppingAction stoppingAction,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            CreateClient(apiToken);

            var account = await WrapApiCallAsync(_dropboxClient.Users.GetCurrentAccountAsync);

            _state = _stateService.GetState(account.AccountId);
            if (_state != null)
                _lastProcessedFileIndex = _state.LastProcessedFileIndex;
            else
                _state = new DropboxDownloadState { Started = DateTimeOffset.Now, AccountId = account.AccountId };

            bool firstIteration = true;
            var filesMetadata = new List<Metadata>();

            var listFolderResult = await WrapApiCallAsync(() => _dropboxClient.Files.ListFolderAsync(CameraUploads, limit: Limit));

            while (listFolderResult.HasMore || firstIteration)
            {
                if (!firstIteration)
                    listFolderResult = await WrapApiCallAsync(() =>
                        _dropboxClient.Files.ListFolderContinueAsync(listFolderResult.Cursor));

                filesMetadata.AddRange(listFolderResult.Entries.Where(a => a is FileMetadata));

                firstIteration = false;
            }

            _state.TotalFiles = filesMetadata.Count;

            var index = _lastProcessedFileIndex > -1 ? _lastProcessedFileIndex : 0;

            for (int i = index; i < filesMetadata.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested || stoppingAction.IsStopRequested)
                {
                    _logger.LogInformation("Cancellation requested.");
                    yield break;
                }

                var fileMetadata = filesMetadata[i];

                var dropboxFile = await DownloadFileAsync(fileMetadata, account);
                if (dropboxFile == null)
                    yield break;

                _state.LastProcessedFileIndex++;
                _state.LastProcessedFileId = dropboxFile.FileId;

                _progressReporter.Report(_state.AccountId, _state.LastProcessedFileIndex, _state.TotalFiles);

                yield return dropboxFile;
            }
        }

        public void Dispose()
        {
            SaveState();
        }

        private async Task<DropboxFile> DownloadFileAsync(Metadata metadata, Account account)
        {
            var metadataName = metadata.Name;

            try
            {
                _logger.LogInformation($"Dropbox: started downloading {metadataName}.");

                var path = CameraUploads + "/" + metadataName;
                var fileMetadata = await WrapApiCallAsync(() => _dropboxClient.Files.DownloadAsync(path));
                var fileContents = await fileMetadata.GetContentAsByteArrayAsync();

                _logger.LogInformation($"Dropbox: finished downloading {metadataName}.");
                _logger.LogInformation($"Dropbox: started saving {metadataName}.");

                var filePath = Path.Combine("Dropbox", account.Email, metadataName);
                var createdOn = fileMetadata.Response.ClientModified;

                var savedFileDto = await _storageService.SaveFileAsync(filePath, fileContents);

                var dropboxFile = new DropboxFile(account.Email, account.AccountId,
                    metadataName, filePath, savedFileDto.Id, path, createdOn, fileMetadata.Response.Id);

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

        private void SaveState()
        {
            _logger.LogInformation("Dropbox: saving state.");
            _logger.LogInformation(
                $"Dropbox: files processed/total - {_state.LastProcessedFileId}/{_state.TotalFiles}");

            _state.LastProcessedFileIndex = _lastProcessedFileIndex;
            _stateService.SaveState(_state);
        }

        private async Task<T> WrapApiCallAsync<T>(Func<Task<T>> apiCall)
        {
            try
            {
                return await apiCall();
            }
            catch (AuthException e)
            {
                if (e.ErrorResponse == AuthError.ExpiredAccessToken.Instance)
                {
                    _logger.LogError("Dropbox: Access token has expired.");
                    throw new DropboxException("Access token has expired.");
                }

                _logger.LogError("Dropbox: " + e.Message);
                throw new DropboxException(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Dropbox: " + e.Message);

                throw new DropboxException(e.Message);
            }
        }
    }
}
