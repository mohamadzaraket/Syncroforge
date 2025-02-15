using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Otp
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
