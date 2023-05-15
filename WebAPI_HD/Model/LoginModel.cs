using System.ComponentModel.DataAnnotations;

namespace WebAPI_HD.Model
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
