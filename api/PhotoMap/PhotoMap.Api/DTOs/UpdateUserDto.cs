using System;
using PhotoMap.Api.Database.Entities;

namespace PhotoMap.Api.DTOs
{
    public class UpdateUserDto
    {
        public string YandexDiskToken { get; set; }
        public int? YandexDiskTokenExpiresIn { get; set; }
        public ProcessingStatus? YandexDiskStatus { get; set; }
        public string DropboxToken { get; set; }
        public int? DropboxTokenExpiresIn { get; set; }
        public ProcessingStatus? DropboxStatus { get; set; }
    }
}
