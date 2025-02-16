using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Comany
{
    public class GetCompanyRequest
    {
        [Required]
        public bool WithEmployee { get; set; }
        [Required]
        public bool WithDepartment { get; set; }
    }
}
