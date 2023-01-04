using System.Data;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
namespace WebAPI_HD.Model
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
       /* public string? Username { get; set; }*/

    /*    [JsonIgnore]
        public string? PasswordHash { get; set; }*/

    }
}
