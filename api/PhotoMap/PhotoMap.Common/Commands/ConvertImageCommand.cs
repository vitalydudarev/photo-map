using System;
using PhotoMap.Messaging.Commands;

namespace PhotoMap.Common.Commands
{
    public class ConvertImageCommand : CommandBase
    {
        public Guid Id { get; set; }

        public byte[] FileContents { get; set; }
    }
}
