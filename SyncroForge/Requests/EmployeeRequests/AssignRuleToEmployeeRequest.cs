using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.EmployeeRequests
{
    public class AssignRuleToEmployeeRequest
    {
        [Required(ErrorMessage = "RuleId is required.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
            ErrorMessage = "RuleId must be a valid UUID.")]
        public string RuleId { get; set; }
    }
}
