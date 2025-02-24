using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.DepartmentEmployee
{
    public class DepartmentEmployeeRequest
    {
        [Required]
        public string EmployeeIdentifier {  get; set; }
        [Required]
        public string DepartmentIdentifier { get; set; }

    }
}
