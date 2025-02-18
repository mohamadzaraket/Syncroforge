using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace SyncroForge.Models
{
    public class Status
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PublicKey { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string BackgroundColor { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }

        public Status()
        {
            PublicKey = Guid.NewGuid().ToString();
            IsDeleted = false;
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            UpdatedAt = CreatedAt;
        }
    }
}
