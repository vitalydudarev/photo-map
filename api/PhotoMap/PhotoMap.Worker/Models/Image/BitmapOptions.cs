namespace PhotoMap.Worker.Models.Image
{
    public class BitmapOptions
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float Dx { get; set; }
        public float Dy { get; set; }
        public int Degrees { get; set; }

        public BitmapOptions(int width, int height, float dx, float dy, int degrees)
        {
            Width = width;
            Height = height;
            Dx = dx;
            Dy = dy;
            Degrees = degrees;
        }
    }
}
