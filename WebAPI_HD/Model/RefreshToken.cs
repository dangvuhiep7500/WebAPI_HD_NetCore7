namespace WebAPI_HD.Model
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? refreshToken { get; set; }
        public DateTime Expires { get; set; }
        //public bool Revoked { get; set; }
    }
}
