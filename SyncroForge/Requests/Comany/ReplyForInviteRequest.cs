using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Comany
{
    public class ReplyForInviteRequest
    {
        [Required(ErrorMessage = "InviteId is required.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
            ErrorMessage = "InviteId must be a valid UUID.")]
        public string InviteId { get; set; }

        [Required(ErrorMessage = "ReplyValue is required.")]
        [Range(1, 2, ErrorMessage = "ReplyValue must be either 1 or 2.")]
        public int ReplyValue { get; set; }
    }
}
