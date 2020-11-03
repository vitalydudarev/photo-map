using System;
using PhotoMap.Api.Database.Entities;

namespace PhotoMap.Api.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string YandexDiskAccessToken { get; set; }

        public DateTime? YandexDiskTokenExpiresOn { get; set; }

        public ProcessingStatus? YandexDiskStatus { get; set; }

        public string DropboxAccessToken { get; set; }

        public DateTime? DropboxTokenExpiresOn { get; set; }

        public ProcessingStatus? DropboxStatus { get; set; }
    }
}
