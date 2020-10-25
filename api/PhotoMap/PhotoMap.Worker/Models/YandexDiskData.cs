using System;

namespace PhotoMap.Worker.Models
{
    public class YandexDiskData
    {
        public int UserId { get; set; }
        public string YandexDiskAccessToken { get; set; }
        public int TotalPhotos { get; set; }
        public int CurrentIndex { get; set; }
        public DateTimeOffset Started { get; set; }
    }
}
