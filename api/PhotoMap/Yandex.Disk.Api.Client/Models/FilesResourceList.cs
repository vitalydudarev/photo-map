namespace Yandex.Disk.Api.Client.Models
{
    public class FilesResourceList
    {
        public Resource[] Items { get; set; }
        public long Limit { get; set; }
        public long Offset { get; set; }
    }
}