using System.Data;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
namespace WebAPI_HD.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        [JsonIgnore]
        public virtual ICollection<Bill>? Bill { get; set; }
    }
}
