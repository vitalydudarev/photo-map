using System;

namespace PhotoMap.Worker
{
    public class YandexDiskException : Exception
    {
        public YandexDiskException(string message) : base(message)
        {
        }
    }
}
