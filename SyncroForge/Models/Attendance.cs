using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Models
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {  get; set; }
        public string PublicKey { get; set; }
        public int EmployeeId {  get; set; }
        public virtual Employee Employee { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt {  get; set; }

        public bool IsDeleted { get; set; }
        public Attendance()
        {
            PublicKey = Guid.NewGuid().ToString();
            CreatedAt= DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            UpdatedAt = CreatedAt;


        }
    }
}
