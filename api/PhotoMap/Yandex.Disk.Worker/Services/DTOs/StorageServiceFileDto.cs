using System;

namespace Yandex.Disk.Worker.Services.DTOs
{
    public class StorageServiceFileDto
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
