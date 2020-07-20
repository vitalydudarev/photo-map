namespace PhotoMap.Api.DTOs
{
    public class AddUserDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AccessToken { get; set; }

        public int TokenExpiresIn { get; set; }
    }
}
