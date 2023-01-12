using System.Data;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
namespace WebAPI_HD.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public ICollection<Bill>? Bill { get; set; }
    }
}
