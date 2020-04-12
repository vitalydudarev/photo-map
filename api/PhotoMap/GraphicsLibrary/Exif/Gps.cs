using System;

namespace GraphicsLibrary.Exif
{
    public class Gps
    {
        public DateTime? DateTimeStamp { get; set; }
        public string LatitudeRef { get; set; }
        public LatLng Latitude { get; set; }
        public string LongitudeRef { get; set; }
        public LatLng Longitude { get; set; }
        public byte? AltitudeRef { get; set; }
        public double? Altitude { get; set; }
        public string SpeedRef { get; set; }
        public double? Speed { get; set; }
        public string ImgDirectionRef { get; set; }
        public double? ImgDirection { get; set; }
        public string DestBearingRef { get; set; }
        public double? DestBearing { get; set; }
        public double? HorizontalPositioningError { get; set; }
    }
}