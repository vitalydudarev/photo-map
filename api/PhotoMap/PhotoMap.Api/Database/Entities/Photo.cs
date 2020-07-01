using System;

namespace PhotoMap.Api.Database.Entities
{
    public class Photo
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public long? PhotoFileId { get; set; }

        public string PhotoUrl { get; set; }

        public long ThumbnailSmallFileId { get; set; }

        public long ThumbnailLargeFileId { get; set; }

        public bool HasExternalPhotoUrl { get; set; }

        public DateTimeOffset DateTimeTaken { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool HasGps { get; set; }

        public string ExifString { get; set; }
    }
}
