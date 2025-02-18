using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Models
{
    public class Count
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PublicKey { get; set; }
        [Column(TypeName = "text")] // Use "ntext" for Unicode text in SQL Server
        public string JsonInfo { get; set; }

        public Count() {
            PublicKey= Guid.NewGuid().ToString();
        }
    }
}
