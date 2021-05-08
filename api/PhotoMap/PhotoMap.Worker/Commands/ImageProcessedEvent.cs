using System;
using System.Collections.Generic;
using PhotoMap.Messaging.Events;
using IUserIdentifier = PhotoMap.Worker.Models.IUserIdentifier;

namespace PhotoMap.Worker.Commands
{
    public class ImageProcessedEvent : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }

        public string FileName { get; set; }

        public string FileSource { get; set; }

        public Dictionary<int, long> Thumbs { get; set; }

        public string Path { get; set; }

        public DateTime? FileCreatedOn { get; set; }

        public DateTime? PhotoTakenOn { get; set; }

        public string ExifString { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
