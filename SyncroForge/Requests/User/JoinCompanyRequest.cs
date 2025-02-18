using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.User
{
    public class JoinCompanyRequest
    {
        [Required(ErrorMessage = "Company Identifier is required.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
            ErrorMessage = "Company Identifier must be a valid UUID.")]
        public string CompanyIdentifier { get; set; }
    }
}
