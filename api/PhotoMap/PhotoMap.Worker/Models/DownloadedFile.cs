using System;

namespace PhotoMap.Worker.Models
{
    public class DownloadedFile
    {
        public string FileName { get; set; }

        public long FileId { get; set; }

        public string FileSource { get; set; }

        public string RelativeFilePath { get; set; }

        public string Path { get; set; }

        public DateTime? FileCreatedOn { get; set; }
    }
}
