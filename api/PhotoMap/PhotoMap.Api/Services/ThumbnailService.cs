using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoMap.Api.Services
{
    public class ThumbnailService : IThumbnailService
    {
        private const string Path = "/Users/vitalydudarau/Downloads/Resized";
        private const string SkipFile = ".DS_Store";

        public IEnumerable<string> GetFiles()
        {
            return Directory.EnumerateFiles(Path)
                .Select(System.IO.Path.GetFileName)
                .Where(a => a != SkipFile);
        }

        public int GetFilesCount()
        {
            return Directory
                .EnumerateFiles(Path)
                .Count(a => a.EndsWith(SkipFile));
        }

        public async Task<byte[]> GetContentsAsync(string fileName)
        {
            return await File.ReadAllBytesAsync(System.IO.Path.Combine(Path, fileName));
        }
    }
}
