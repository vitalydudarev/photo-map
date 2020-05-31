using System;

namespace Storage.Service.Models
{
    public class OutgoingFile
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime AddedOn { get; set; }
    }
}