using System;

namespace PhotoMap.Api.DTOs
{
    public class PhotoDto
    {
        public int Id { get; set; }

        public string PhotoUrl { get; set; }

        public long ThumbnailSmallFileId { get; set; }

        public long ThumbnailLargeFileId { get; set; }

        public DateTime DateTimeTaken { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string FileName { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}
