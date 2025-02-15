using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Guest
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, contain one uppercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name must contain only letters.")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Last name must contain only letters.")]
        public string LastName { get; set; }

        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Phone number must be 10 to 15 digits and can include a leading '+'.")]
        public string? PhoneNumber { get; set; }

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be exactly 6 digits.")]
        public int Otp { get; set; }
    }
}