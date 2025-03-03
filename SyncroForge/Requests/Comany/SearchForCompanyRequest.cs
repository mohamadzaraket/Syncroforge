using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Comany
{
    public class SearchForCompanyRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
