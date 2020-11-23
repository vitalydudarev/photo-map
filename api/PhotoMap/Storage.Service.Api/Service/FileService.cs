using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Storage.Service.Database.Entities;
using Storage.Service.Database.Repository;
using Storage.Service.Models;
using Storage.Service.Storage;

namespace Storage.Service.Service
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _repository;
        private readonly IFileStorage _fileStorage;
        private readonly IMapper _mapper;
        private readonly ILogger<FileService> _logger;

        public FileService(
            IFileRepository repository,
            IFileStorage fileStorage,
            IMapper mapper,
            ILogger<FileService> logger)
        {
            _repository = repository;
            _fileStorage = fileStorage;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OutgoingFile> SaveAsync(string fileName, byte[] fileContents)
        {
            try
            {
                var filePath = await _fileStorage.SaveAsync(fileName, fileContents);

                var incomingFile = new IncomingFile
                {
                    AddedOn = DateTime.UtcNow,
                    FileName = fileName,
                    Path = filePath,
                    Size = fileContents.Length
                };

                var incomingFileEntity = _mapper.Map<File>(incomingFile);

                var outgoingFileEntity = await _repository.AddAsync(incomingFileEntity);

                var outgoingFile = _mapper.Map<OutgoingFile>(outgoingFileEntity);

                return outgoingFile;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unable to save {fileName}: {e.Message}");
                throw;
            }
        }

        public async Task<byte[]> GetFileContentsAsync(long fileId)
        {
            try
            {
                var fileEntity = await _repository.GetAsync(fileId);
                if (fileEntity != null)
                {
                    var fileContents = await _fileStorage.GetAsync(fileEntity.FileName);

                    return fileContents;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unable to get {fileId}: {e.Message}");
                throw;
            }

            return null;
        }

        public async Task<OutgoingFileInfo> GetFileInfoAsync(long fileId)
        {
            try
            {
                var fileEntity = await _repository.GetAsync(fileId);
                if (fileEntity != null)
                {
                    return _mapper.Map<OutgoingFileInfo>(fileEntity);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unable to load file info for fileId {fileId}: {e.Message}");
                throw;
            }

            return null;
        }

        public async Task<OutgoingFileInfo> GetFileInfoByFileNameAsync(string fileName)
        {
            try
            {
                var fileEntity = await _repository.GetByFileNameAsync(fileName);
                if (fileEntity != null)
                {
                    return _mapper.Map<OutgoingFileInfo>(fileEntity);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unable to load file info for fileName {fileName}: {e.Message}");
                throw;
            }

            return null;
        }

        public async Task DeleteFileAsync(long fileId)
        {
            try
            {
                var fileEntity = await _repository.GetAsync(fileId);
                if (fileEntity != null)
                {
                    _fileStorage.Delete(fileEntity.FileName);
                    await _repository.DeleteAsync(fileId);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unable to delete file for fileId {fileId}: {e.Message}");
                throw;
            }
        }

        public async Task DeleteAllFilesAsync()
        {
            try
            {
                var files = await _repository.GetAllAsync();

                foreach (var file in files)
                {
                    _fileStorage.Delete(file.FileName);
                }

                await _repository.DeleteAllAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unable to delete all files: {e.Message}");
                throw;
            }
        }
    }
}
