using System.ComponentModel.DataAnnotations;

namespace WebAPI_HD.Model
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
