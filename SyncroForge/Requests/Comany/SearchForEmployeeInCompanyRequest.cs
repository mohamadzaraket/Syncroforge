using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Comany
{
    public class SearchForEmployeeInCompanyRequest
    {
        [Required]
        public string Email { get; set; }

        [Required(ErrorMessage = "Company Identifier is required.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
          ErrorMessage = "Company Identifier must be a valid UUID.")]
        public string CompanyIdentifier { get; set; }
    }
}
