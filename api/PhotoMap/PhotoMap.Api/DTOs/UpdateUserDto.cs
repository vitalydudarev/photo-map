using System;
using PhotoMap.Api.Database.Entities;

namespace PhotoMap.Api.DTOs
{
    public class UpdateUserDto
    {
        public string YandexDiskToken { get; set; }
        public DateTimeOffset? YandexDiskTokenExpiresOn { get; set; }
        public ProcessingStatus? YandexDiskStatus { get; set; }
        public string DropboxToken { get; set; }
        public DateTimeOffset? DropboxTokenExpiresOn { get; set; }
        public ProcessingStatus? DropboxStatus { get; set; }
    }
}
