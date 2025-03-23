using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Attachmet
{
    public class ADDAttachmetRequest
    {
        [Required]
     
        public IFormFile file { get; set; }
    }
}
