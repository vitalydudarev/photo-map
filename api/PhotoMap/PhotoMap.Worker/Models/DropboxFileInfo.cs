using System;

namespace PhotoMap.Worker.Models
{
    public class DropboxFileInfo : DownloadedFileInfo
    {
        public override string Source { get; set; } = "Dropbox";

        public string FileId { get; set; }

        public DropboxFileInfo(string resourceName, string path, DateTime? createdOn, string fileId, string userName,
            byte[] fileContents)
            : base(resourceName, path, createdOn, userName, fileContents)
        {
            FileId = fileId;
        }
    }
}
