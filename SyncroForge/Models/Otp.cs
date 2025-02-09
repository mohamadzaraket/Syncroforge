using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Models
{
    public class Otp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PublicKey { get; set; }
        public int OtpNumber { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber {  get; set; }
        public long ExpiryAt { get; set; }
        public long UpdatedAt { get; set; }
        public long CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string type {  get; set; }


        public Otp()
        {
            PublicKey = Guid.NewGuid().ToString();
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            UpdatedAt = CreatedAt;
            ExpiryAt = CreatedAt + 600;


        }
    }
}
