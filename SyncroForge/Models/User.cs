using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncroForge.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PublicKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileUrl { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public bool TwoFa {get;set;}
        public bool IsDeleted { get; set;}
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection< Employee> Employees { get; set; }
        public virtual ICollection< Company> CreatedCompanies { get; set; }
        public virtual ICollection<CompanyInviteUser> Invites {  get; set; }

        public User()
        {
            PublicKey = Guid.NewGuid().ToString();
            IsDeleted = false;
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            UpdatedAt = CreatedAt;
        }

    }
}
