using System;

namespace GraphicsLibrary.Exif
{
    public class Ifd
    {
        public DateTime? DateTimeOriginal { get; set; }
        public DateTime? DateTimeDigitized { get; set; }
        public string TimeZone { get; set; }
        public string TimeZoneOriginal { get; set; }
        public string TimeZoneDigitized { get; set; }
    }
}
