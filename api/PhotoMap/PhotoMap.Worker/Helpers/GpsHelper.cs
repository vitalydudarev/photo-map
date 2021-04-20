using LatLng = PhotoMap.Worker.Models.Image.LatLng;

namespace PhotoMap.Worker.Helpers
{
    public static class GpsHelper
    {
        public static double ConvertLatitude(LatLng latLng, string latitudeRef)
        {
            int multiplier = latitudeRef == "S" ? -1 : 1;
            return multiplier * (latLng.Degrees + latLng.Minutes / 60 + latLng.Seconds / 3600);
        }

        public static double ConvertLongitude(LatLng latLng, string longitudeRef)
        {
            int multiplier = longitudeRef == "W" ? -1 : 1;
            return multiplier * (latLng.Degrees + latLng.Minutes / 60 + latLng.Seconds / 3600);
        }
    }
}
