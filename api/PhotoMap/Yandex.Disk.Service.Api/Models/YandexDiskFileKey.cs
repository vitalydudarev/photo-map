namespace Yandex.Disk.Service.Api.Models
{
    public class YandexDiskFileKey
    {
        public string Login { get; set; }
        public string Uid { get; set; }
        public string Name { get; set; }

        public YandexDiskFileKey(string login, string uid, string name)
        {
            Login = login;
            Uid = uid;
            Name = name;
        }

        public override string ToString()
        {
            return $"{Login}-{Uid}-{Name}";
        }
    }
}