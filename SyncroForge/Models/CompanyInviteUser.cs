using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncroForge.Models
{
    public class CompanyInviteUser
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PublicKey { get; set; }

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int status { get; set; }//0 still not responded 1 acceptedf 2 rejected
        public bool joinedByUser { get; set; }//if false thats mean invited by comany if true user requested to join
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }

        public CompanyInviteUser()
        {
            PublicKey = Guid.NewGuid().ToString();
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            UpdatedAt = CreatedAt;
            status = 0;
        }


    }
}
