using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SyncroForge.Models
{
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PublicKey { get; set; }
        public string Token {  get; set; }
        public long ExpiryAt { get; set; }
        public long UpdatedAt { get; set; }
        public long CreatedAt { get; set; }
        public bool IsDeleted {  get; set; }
        public int UserId {  get; set; }
        public virtual User User { get; set; }


        public RefreshToken()
        {
            PublicKey = Guid.NewGuid().ToString();
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            UpdatedAt = CreatedAt;
            ExpiryAt = CreatedAt + 604800 ;
            

        }

    }
}
