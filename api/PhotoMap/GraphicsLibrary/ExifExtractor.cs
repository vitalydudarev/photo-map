using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GraphicsLibrary.Exif;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Directory = MetadataExtractor.Directory;

namespace GraphicsLibrary
{
    public class ExifExtractor
    {
        public ExifData GetDataAsync(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            
            var data = ImageMetadataReader.ReadMetadata(stream);
            
            return new ExifData { Ifd = ParseIfd(data), Gps = ParseGps(data) };
        }

        private Gps ParseGps(IEnumerable<Directory> data)
        {
            var gpsDirectory = data.OfType<GpsDirectory>().FirstOrDefault();
            if (gpsDirectory != null)
            {
                return new Gps
                {
                    Altitude = ParseRational(gpsDirectory, GpsDirectory.TagAltitude, Convert.ToDouble),
                    AltitudeRef = ParseByte(gpsDirectory, GpsDirectory.TagAltitudeRef),
                    DateTimeStamp = ParseDateTime(gpsDirectory),
                    DestBearing = ParseRational(gpsDirectory, GpsDirectory.TagDestBearing, Convert.ToDouble),
                    DestBearingRef = ParseString(gpsDirectory, GpsDirectory.TagDestBearingRef),
                    HorizontalPositioningError = ParseRational(gpsDirectory, GpsDirectory.TagHPositioningError,
                        Convert.ToDouble),
                    ImgDirection = ParseRational(gpsDirectory, GpsDirectory.TagImgDirection, Convert.ToDouble),
                    ImgDirectionRef = ParseString(gpsDirectory, GpsDirectory.TagImgDirectionRef),
                    Latitude = ParseLatLng(gpsDirectory.GetRationalArray(GpsDirectory.TagLatitude)),
                    LatitudeRef = ParseString(gpsDirectory, GpsDirectory.TagLatitudeRef),
                    Longitude = ParseLatLng(gpsDirectory.GetRationalArray(GpsDirectory.TagLongitude)),
                    LongitudeRef = ParseString(gpsDirectory, GpsDirectory.TagLongitudeRef),
                    Speed = ParseRational(gpsDirectory, GpsDirectory.TagSpeed, Convert.ToDouble),
                    SpeedRef = ParseString(gpsDirectory, GpsDirectory.TagSpeedRef)
                };
            }

            return null;
        }
        
        private Ifd ParseIfd(IEnumerable<Directory> data)
        {
            var subIfd = data.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (subIfd != null)
            {
                return new Ifd
                {
                    DateTimeDigitized = ParseString(subIfd, ExifDirectoryBase.TagDateTimeDigitized),
                    DateTimeOriginal = ParseString(subIfd, ExifDirectoryBase.TagDateTimeOriginal),
                    TimeZone = ParseString(subIfd, ExifDirectoryBase.TagTimeZone),
                    TimeZoneOriginal = ParseString(subIfd, ExifDirectoryBase.TagTimeZoneOriginal),
                    TimeZoneDigitized = ParseString(subIfd, ExifDirectoryBase.TagTimeZoneDigitized)
                };
            }

            return null;
        }

        private static string ParseString(Directory subIfd, int tag)
        {
            var stringValue = subIfd.GetStringValue(tag);
            if (stringValue.Bytes != null)
                return stringValue.ToString();

            return null;
        }
        
        private static byte? ParseByte(Directory subIfd, int tag)
        {
            if (subIfd.TryGetByte(tag, out var byteValue))
                return byteValue;

            return null;
        }
        
        private static T ParseRational<T>(Rational rational)
        {
            // causes boxing and unboxing
            var orig = (rational.Numerator / (double) rational.Denominator);
            var boxed = (object) orig;
            var unboxed = (T) boxed;
            return (T)(object)(rational.Numerator / (double) rational.Denominator);
        }
        
        private static T ParseRational<T>(Rational rational, Func<object, T> convert)
        {
            // causes boxing and unboxing
            var orig = (rational.Numerator / (double) rational.Denominator);
            var boxed = (object) orig;

            return convert(boxed);
            
            var unboxed = (T) boxed;
            return (T)(object)(rational.Numerator / (double) rational.Denominator);
        }
        
        private static T? ParseRational<T>(Directory directory, int tag, Func<double, T> convert) where T : struct
        {
            if (directory.TryGetRational(tag, out var rational))
            {
                var orig = (rational.Numerator / (double) rational.Denominator);
                // var boxed = (object) orig;

                return convert(orig);
            }

            return null;
        }

        private static LatLng ParseLatLng(Rational[]? array)
        {
            if (array != null)
            {
                var degrees = ParseRational<double>(array[0]);
                var minutes = ParseRational<double>(array[1]);
                var seconds = ParseRational<double>(array[2]);

                return new LatLng { Degrees = degrees, Minutes = minutes, Seconds = seconds };
            }

            return null;
        }

        private static DateTime? ParseDateTime(Directory gpsDirectory)
        {
            var timeStamp = gpsDirectory.GetRationalArray(GpsDirectory.TagTimeStamp);
            var dateStamp = gpsDirectory.GetString(GpsDirectory.TagDateStamp);
            
            if (timeStamp != null && dateStamp != null)
            {
                var hour = ParseRational(timeStamp[0], Convert.ToInt32);
                var minute = ParseRational(timeStamp[1], Convert.ToInt32);
                var second = ParseRational(timeStamp[2], Convert.ToInt32);

                static string Format(int i) => i < 10 ? $"0{i}" : i.ToString();

                var dateTimeStr = $"{dateStamp} {Format(hour)}:{Format(minute)}:{Format(second)}Z";
                const string dateTimeFormat = "yyyy:MM:dd HH:mm:ssZ";

                if (DateTime.TryParseExact(dateTimeStr, dateTimeFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var dateTime))
                    return dateTime;
            }

            return null;
        }
    }
}