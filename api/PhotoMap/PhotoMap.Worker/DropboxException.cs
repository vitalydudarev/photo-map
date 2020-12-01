using System;

namespace PhotoMap.Worker
{
    public class DropboxException : Exception
    {
        public DropboxException(string message) : base(message)
        {
        }
    }
}
