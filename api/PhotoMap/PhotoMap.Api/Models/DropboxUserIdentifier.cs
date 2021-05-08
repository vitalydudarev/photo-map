namespace PhotoMap.Api.Models
{
    public class DropboxUserIdentifier : IUserIdentifier
    {
        public int UserId { get; set; }

        public string GetKey()
        {
            return "Dropbox." + UserId;
        }
    }
}
