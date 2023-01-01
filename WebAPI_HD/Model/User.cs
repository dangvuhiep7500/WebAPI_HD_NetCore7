using System.Data;
using System.Text.Json.Serialization;

namespace WebAPI_HD.Model
{
    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }

        [JsonIgnore]
        public string? PasswordHash { get; set; }

    }
}
