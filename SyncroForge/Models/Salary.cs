using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncroForge.Models
{
    public class Salary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public float Ammount { get; set; }
        public string PublicKey { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Salary()
        {
            PublicKey = Guid.NewGuid().ToString();
            IsDeleted = false;
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            UpdatedAt = CreatedAt;
        }

    }
}
