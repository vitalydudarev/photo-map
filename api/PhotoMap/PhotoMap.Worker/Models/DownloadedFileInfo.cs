using System;

namespace PhotoMap.Worker.Models
{
    public abstract class DownloadedFileInfo
    {
        public string ResourceName { get; set; }

        public string Path { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string UserName { get; set; }

        public byte[] FileContents { get; set; }

        public abstract string Source { get; set; }

        protected DownloadedFileInfo(string resourceName, string path, DateTime? createdOn, string userName,
            byte[] fileContents)
        {
            ResourceName = resourceName;
            Path = path;
            CreatedOn = createdOn;
            UserName = userName;
            FileContents = fileContents;
        }
    }
}
