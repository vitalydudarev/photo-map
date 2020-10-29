using System;

namespace PhotoMap.Api.Database.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string YandexDiskToken { get; set; }

        public DateTimeOffset? YandexDiskTokenExpiresOn { get; set; }

        public ProcessingStatus? YandexDiskStatus { get; set; }

        public string DropboxToken { get; set; }

        public DateTimeOffset? DropboxTokenExpiresOn { get; set; }

        public ProcessingStatus? DropboxStatus { get; set; }
    }
}
