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
            using var stream = new MemoryStream(bytes);

            var data = ImageMetadataReader.ReadMetadata(stream);

            return new ExifData
            {
                ExifSubIfd = ParseExifSubIfd(data.ToList()),
                ExifIfd0 = ParseExifIfd0(data.ToList()),
                Gps = ParseGps(data)
            };
        }

        private static Gps ParseGps(IEnumerable<Directory> data)
        {
            var gpsDirectory = data.OfType<GpsDirectory>().FirstOrDefault();
            if (gpsDirectory != null)
            {
                return new Gps
                {
                    Altitude = ParseRational(gpsDirectory, GpsDirectory.TagAltitude, Convert.ToDouble),
                    AltitudeRef = ParseByte(gpsDirectory, GpsDirectory.TagAltitudeRef),
                    DateTimeStamp = ParseDateTime(gpsDirectory, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal),
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

        private static ExifSubIfd ParseExifSubIfd(IList<Directory> data)
        {
            var subIfd = data.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (subIfd != null)
            {
                return new ExifSubIfd
                {
                    DateTimeDigitized = ParseDateTime(ParseString(subIfd, ExifDirectoryBase.TagDateTimeDigitized), DateTimeStyles.AssumeLocal),
                    DateTimeOriginal = ParseDateTime(ParseString(subIfd, ExifDirectoryBase.TagDateTimeOriginal), DateTimeStyles.AssumeLocal),
                    TimeZone = ParseString(subIfd, ExifDirectoryBase.TagTimeZone),
                    TimeZoneOriginal = ParseString(subIfd, ExifDirectoryBase.TagTimeZoneOriginal),
                    TimeZoneDigitized = ParseString(subIfd, ExifDirectoryBase.TagTimeZoneDigitized),
                    Width = ParseInt(subIfd, ExifDirectoryBase.TagExifImageWidth),
                    Height = ParseInt(subIfd, ExifDirectoryBase.TagExifImageHeight)
                };
            }

            return null;
        }

        private static ExifIfd0 ParseExifIfd0(IList<Directory> data)
        {
            var ifd = data.OfType<ExifIfd0Directory>().FirstOrDefault();
            if (ifd != null)
            {
                return new ExifIfd0
                {
                    Make = ParseString(ifd, ExifDirectoryBase.TagMake),
                    Model = ParseString(ifd, ExifDirectoryBase.TagModel),
                    Software = ParseString(ifd, ExifDirectoryBase.TagSoftware)
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

        private static int? ParseInt(Directory directory, int tag)
        {
            if (directory.TryGetInt32(tag, out var intValue))
                return intValue;

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

        private static DateTime? ParseDateTime(Directory gpsDirectory, DateTimeStyles dateTimeStyles)
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

                return ParseDateTime(dateTimeStr, dateTimeStyles);
            }

            return null;
        }

        private static DateTime? ParseDateTime(string dateTimeStr, DateTimeStyles dateTimeStyles)
        {
            const string dateTimeFormat1 = "yyyy:MM:dd HH:mm:ssZ";
            const string dateTimeFormat2 = "yyyy:MM:dd HH:mm:ss";

            if (DateTime.TryParseExact(dateTimeStr, new [] { dateTimeFormat1, dateTimeFormat2 }, CultureInfo.InvariantCulture,
                dateTimeStyles, out var dateTime))
                return dateTime;

            return null;
        }
    }
}
