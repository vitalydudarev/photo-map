namespace Yandex.Disk.Worker
{
    public class ImageProcessingSettings
    {
        public bool DeleteAfterProcessing { get; set; }
        public int[] Sizes { get; set; }
    }
}
