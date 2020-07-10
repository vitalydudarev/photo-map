namespace PhotoMap.Messaging
{
    public class RabbitMqConfiguration
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string HostName { get; set; }

        public int Port { get; set; }

        public string ConsumeQueueName { get; set; }

        public string ResponseQueueName { get; set; }
    }
}
