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
    }
}
