using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoMap.Api.Database.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public long? PhotoFileId { get; set; }

        public long ThumbnailSmallFileId { get; set; }

        public long ThumbnailLargeFileId { get; set; }

        public string FileName { get; set; }

        public DateTimeOffset DateTimeTaken { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool HasGps { get; set; }

        public string ExifString { get; set; }

        public string Source { get; set; }

        public string Path { get; set; }

        public DateTimeOffset AddedOn { get; set; }
    }
}
