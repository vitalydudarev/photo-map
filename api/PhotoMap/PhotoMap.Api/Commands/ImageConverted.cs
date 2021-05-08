using System;
using PhotoMap.Messaging.Events;

namespace PhotoMap.Api.Commands
{
    public class ImageConverted : EventBase
    {
        public Guid Id { get; set; }

        public byte[] FileContents { get; set; }
    }
}
