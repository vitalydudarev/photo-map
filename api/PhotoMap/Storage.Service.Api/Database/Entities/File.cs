using System;

namespace Storage.Service.Database.Entities
{
    public class File
    {
        public long Id { set; get; }
        public string FileName { set; get; }
        public string FullPath { set; get; }
        public DateTime AddedOn { set; get; }
        public long Size { set; get; }
    }
}