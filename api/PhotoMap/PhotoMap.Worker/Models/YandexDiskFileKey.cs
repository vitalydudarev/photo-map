using System;

namespace PhotoMap.Worker.Models
{
    public class YandexDiskFileKey
    {
        public string Login { get; set; }
        public string Uid { get; set; }
        public string ResourceName { get; set; }
        public string RelativeFilePath { get; set; }
        public long StorageFileId { get; set; }
        public string Path { get; set; }
        public DateTime? CreatedOn { get; set; }

        public YandexDiskFileKey(string login, string uid, string resourceName, string relativeFilePath, long storageFileId,
            string path, DateTime? createdOn)
        {
            Login = login;
            Uid = uid;
            ResourceName = resourceName;
            RelativeFilePath = relativeFilePath;
            StorageFileId = storageFileId;
            Path = path;
            CreatedOn = createdOn;
        }

        public override string ToString()
        {
            return $"{Login}-{Uid}-{ResourceName}";
        }
    }
}
