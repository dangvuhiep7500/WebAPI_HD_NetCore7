namespace WebAPI_HD.Model
{
    public class UserWithToken : User
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public UserWithToken(User user)
        {
            UserId = user.UserId;
            EmailAddress = user.EmailAddress;
            Username = user.Username;
            FirstName = user.FirstName;
            MiddleName = user.MiddleName;
            LastName = user.LastName;
            HireDate = user.HireDate;
            Role = user.Role;
        }
    }
}
