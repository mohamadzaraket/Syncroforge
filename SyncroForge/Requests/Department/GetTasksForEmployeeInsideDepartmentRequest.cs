using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Department
{
    public class GetTasksForEmployeeInsideDepartmentRequest
    {
        [Required(ErrorMessage = "DepartmentId is required.")]
        [RegularExpression(
    @"^(\{)?[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[1-5][0-9a-fA-F]{3}\-[89abAB][0-9a-fA-F]{3}\-[0-9a-fA-F]{12}(\})?$",
    ErrorMessage = "DepartmentId must be a valid GUID.")]
        public string DepartmentId { get; set; }
    }
}
