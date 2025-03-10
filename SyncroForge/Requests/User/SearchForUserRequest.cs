using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.User
{
    public class SearchForUserRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
