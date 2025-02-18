using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Department
{
    public class GetDepartmentRequest
    {
        [Required]
        public bool WithEmployee { get; set; }
        [Required]
        public bool WithTask { get; set; }
    }
}
