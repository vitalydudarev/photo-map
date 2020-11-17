using System;

namespace PhotoMap.Worker.Models
{
    public class DropboxFile
    {
        public string Email { get; set; }
        public string AccountId { get; set; }
        public string ResourceName { get; set; }
        public string RelativeFilePath { get; set; }
        public long StorageFileId { get; set; }
        public string Path { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string FileId { get; set; }

        public DropboxFile(string email, string accountId, string resourceName, string relativeFilePath,
            long storageFileId, string path, DateTime? createdOn, string fileId)
        {
            Email = email;
            AccountId = accountId;
            ResourceName = resourceName;
            RelativeFilePath = relativeFilePath;
            StorageFileId = storageFileId;
            Path = path;
            CreatedOn = createdOn;
            FileId = fileId;
        }

        public override string ToString()
        {
            return $"{Email}-{AccountId}-{ResourceName}";
        }
    }
}
