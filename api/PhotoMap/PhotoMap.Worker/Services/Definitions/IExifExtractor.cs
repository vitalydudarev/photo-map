using PhotoMap.Worker.Models.Image;

namespace PhotoMap.Worker.Services.Definitions
{
    public interface IExifExtractor
    {
        ExifData GetDataAsync(byte[] bytes);
    }
}
