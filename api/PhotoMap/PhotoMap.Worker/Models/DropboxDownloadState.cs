using System;

namespace PhotoMap.Worker.Models
{
    public class DropboxDownloadState
    {
        public string AccountId { get; set; }
        public string AccessToken { get; set; }
        public int TotalFiles { get; set; }
        public int LastProcessedFileIndex { get; set; }
        public string LastProcessedFileId { get; set; }
        public DateTimeOffset Started { get; set; }
    }
}
