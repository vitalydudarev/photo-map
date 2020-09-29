using System;
using System.Collections.Generic;
using GraphicsLibrary.Exif;

namespace PhotoMap.Messaging.Commands
{
    public class ResultsCommand : CommandBase
    {
        public int UserId { get; set; }

        public long? FileId { get; set; }

        public string FileName { get; set; }

        public string FileSource { get; set; }

        public Dictionary<int, long> Thumbs { get; set; }

        public string PhotoUrl { get; set; }

        public string Path { get; set; }

        public DateTime? FileCreatedOn { get; set; }

        public DateTime? PhotoTakenOn { get; set; }

        public string ExifString { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
