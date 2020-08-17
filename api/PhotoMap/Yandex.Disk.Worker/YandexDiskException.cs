using System;

namespace Yandex.Disk.Worker
{
    public class YandexDiskException : Exception
    {
        public YandexDiskException(string message) : base(message)
        {
        }
    }
}
