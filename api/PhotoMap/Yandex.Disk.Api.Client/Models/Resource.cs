using System;
using System.Text.Json.Serialization;

namespace Yandex.Disk.Api.Client.Models
{
    public class Resource
    {
        public string AntivirusStatus { get; set; }
        public string ResourceId { get; set; }
        public ShareInfo Share { get; set; }
        public string File { get; set; }
        public long Size { get; set; }
        public DateTime? PhotosliceTime { get; set; }
        [JsonPropertyName("_embedded")]
        public ResourceList Embedded { get; set; }
        public Exif Exif { get; set; }
        public string CustomProperties { get; set; }
        public string MediaType { get; set; }
        public string Preview { get; set; }
        public string Type { get; set; }
        public string MimeType { get; set; }
        public long Revision { get; set; }
        public string PublicUrl { get; set; }
        public string Path { get; set; }
        public string Md5 { get; set; }
        public string PublicKey { get; set; }
        public string Sha256 { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public CommentIds CommentIds { get; set; }
    }
}
