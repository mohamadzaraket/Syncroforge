using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Comany
{
    public class GetSalariesRequest
    {
        [Required]
        [RegularExpression(@"^[{(]?[0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12}[)}]?$",
            ErrorMessage = "companyId must be a valid UUID")]
        public string companyId {  get; set; }
    }
}
