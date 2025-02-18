using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Guest
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Or Password")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$",
    ErrorMessage = "Invalid Email Or Password")]
        public string Password { get; set; }
    }
}
