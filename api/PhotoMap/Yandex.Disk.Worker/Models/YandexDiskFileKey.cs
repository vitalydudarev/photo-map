using System;

namespace Yandex.Disk.Worker.Models
{
    public class YandexDiskFileKey
    {
        public string Login { get; set; }
        public string Uid { get; set; }
        public string Name { get; set; }
        public string RelativeFilePath { get; set; }
        public long StorageFileId { get; set; }
        public string FileUrl { get; set; }
        public string Path { get; set; }
        public DateTime? CreatedOn { get; set; }

        public YandexDiskFileKey(string login, string uid, string name, string relativeFilePath, long storageFileId,
            string fileUrl, string path, DateTime? createdOn)
        {
            Login = login;
            Uid = uid;
            Name = name;
            RelativeFilePath = relativeFilePath;
            StorageFileId = storageFileId;
            FileUrl = fileUrl;
            Path = path;
            CreatedOn = createdOn;
        }

        public override string ToString()
        {
            return $"{Login}-{Uid}-{Name}";
        }
    }
}
