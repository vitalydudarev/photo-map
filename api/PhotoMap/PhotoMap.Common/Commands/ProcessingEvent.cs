using System;
using PhotoMap.Common.Models;
using PhotoMap.Messaging.Events;

namespace PhotoMap.Common.Commands
{
    public class ProcessingEvent : EventBase
    {
        public IUserIdentifier UserIdentifier { get; set; }

        public string FileName { get; set; }

        public long FileId { get; set; }

        public string FileUrl { get; set; }

        public string FileSource { get; set; }

        public bool DeleteAfterProcessing { get; set; }

        public int[] Sizes { get; set; }

        public string RelativeFilePath { get; set; }

        public string Path { get; set; }

        public DateTime? FileCreatedOn { get; set; }
    }
}
