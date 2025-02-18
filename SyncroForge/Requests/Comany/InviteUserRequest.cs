using System;
using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Company
{
    public class InviteUserRequest
    {
        [Required(ErrorMessage = "Company Identifier is required.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
            ErrorMessage = "Company Identifier must be a valid UUID.")]
        public string CompanyIdentifier { get; set; }

        [Required(ErrorMessage = "User Identifier is required.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
            ErrorMessage = "User Identifier must be a valid UUID.")]
        public string UserIdentifier { get; set; }
    }
}
