namespace PhotoMap.Api.Models
{
    public interface IUserIdentifier
    {
        int UserId { get; set; }
        string GetKey();
    }
}
