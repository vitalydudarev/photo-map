namespace PhotoMap.Worker.Models
{
    public interface IUserIdentifier
    {
        int UserId { get; set; }
        string GetKey();
    }
}
