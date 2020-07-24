using System.Collections.Generic;
using GraphicsLibrary.Exif;

namespace PhotoMap.Messaging.Commands
{
    public class ResultsCommand : CommandBase
    {
        public int UserId { get; set; }

        public long FileId { get; set; }

        public ExifData Exif { get; set; }

        public Dictionary<int, long> ThumbsSizes { get; set; }

        public string PhotoUrl { get; set; }
    }
}
