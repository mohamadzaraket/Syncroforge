using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Department
{
    public class GetDepartmentUserRequest
    {
        [Required]
        public string CompanyId { get; set; }
    }
}
