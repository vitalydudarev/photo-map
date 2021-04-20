using System;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Common.Commands
{
    public class ImageConverted : CommandBase
    {
        public Guid Id { get; set; }

        public byte[] FileContents { get; set; }
    }
}
