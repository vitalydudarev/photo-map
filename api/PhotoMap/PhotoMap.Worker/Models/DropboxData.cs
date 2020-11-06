using System;

namespace PhotoMap.Worker.Models
{
    public class DropboxData
    {
        public string AccountId { get; set; }
        public string AccessToken { get; set; }
        public int TotalFiles { get; set; }
        public int CurrentIndex { get; set; }
        public DateTimeOffset Started { get; set; }
    }
}
