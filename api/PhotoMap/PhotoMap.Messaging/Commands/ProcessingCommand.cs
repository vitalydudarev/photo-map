namespace PhotoMap.Messaging.Commands
{
    public class ProcessingCommand : CommandBase
    {
        public int UserId { get; set; }

        public long FileId { get; set; }

        public string FileUrl { get; set; }

        public bool DeleteAfterProcessing { get; set; }

        public int[] Sizes { get; set; }

        public string RelativeFilePath { get; set; }
    }
}