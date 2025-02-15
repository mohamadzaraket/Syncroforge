using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Guest
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "RefreshToken is required.")]
        public string RefreshToken { get; set; }
    }
}
