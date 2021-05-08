namespace PhotoMap.Api.Models
{
    public class YandexDiskUserIdentifier : Api.Models.IUserIdentifier
    {
        public int UserId { get; set; }

        public string GetKey()
        {
            return "Yandex.Disk." + UserId;
        }
    }
}
