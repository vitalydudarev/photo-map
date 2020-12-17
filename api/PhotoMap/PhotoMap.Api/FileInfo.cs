using System;

namespace PhotoMap.Api
{
    public class FileInfo
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
