using System;

namespace PhotoMap.Api.Database.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string YandexDiskToken { get; set; }

        public DateTimeOffset YandexDiskTokenExpiresOn { get; set; }

        public YandexDiskStatus YandexDiskStatus { get; set; }
    }
}
