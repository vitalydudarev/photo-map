namespace PhotoMap.Common.Models
{
    public interface IUserIdentifier
    {
        int UserId { get; set; }
        string GetKey();
    }
}
