using System;

namespace PhotoMap.Worker.Services.DTOs
{
    public class StorageServiceFileDto
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
