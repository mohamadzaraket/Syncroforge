using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Comany
{
    public class SearchForEmployeeRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
