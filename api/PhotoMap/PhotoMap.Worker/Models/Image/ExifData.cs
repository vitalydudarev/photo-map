namespace PhotoMap.Worker.Models.Image
{
    public class ExifData
    {
        public ExifSubIfd ExifSubIfd { get; set; }
        public Gps Gps { get; set; }
        public ExifIfd0 ExifIfd0 { get; set; }
    }
}
