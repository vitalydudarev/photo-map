namespace Image.Service
{
    public class RabbitMqSettings
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string HostName { get; set; }

        public int Port { get; set; }

        public string ProcessingQueueName { get; set; }

        public string ResultsQueueName { get; set; }
    }
}
