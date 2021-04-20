using System;

namespace PhotoMap.Worker.Models.Image
{
    public class ExifSubIfd
    {
        public DateTime? DateTimeOriginal { get; set; }
        public DateTime? DateTimeDigitized { get; set; }
        public string TimeZone { get; set; }
        public string TimeZoneOriginal { get; set; }
        public string TimeZoneDigitized { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}
