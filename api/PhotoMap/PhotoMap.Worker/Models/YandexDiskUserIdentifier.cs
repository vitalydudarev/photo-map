namespace PhotoMap.Worker.Models
{
    public class YandexDiskUserIdentifier : Worker.Models.IUserIdentifier
    {
        public int UserId { get; set; }

        public string GetKey()
        {
            return "Yandex.Disk." + UserId;
        }
    }
}
