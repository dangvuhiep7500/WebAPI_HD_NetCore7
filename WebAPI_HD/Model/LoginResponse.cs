namespace WebAPI_HD.Model
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Token { get; set; }
    }
}
