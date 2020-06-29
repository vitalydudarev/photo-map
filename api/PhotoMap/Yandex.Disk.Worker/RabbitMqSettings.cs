namespace Yandex.Disk.Worker
{
    public class RabbitMqSettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public string IncomingQueueName { get; set; }
        public string OutgoingQueueName { get; set; }
    }
}
