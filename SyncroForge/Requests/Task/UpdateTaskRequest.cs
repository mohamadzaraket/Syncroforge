using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Task
{
    public class UpdateTaskRequest
    {
        [Required]
        public string TaskIdentifier { get; set; }
        public string? ParentTaskIdentifier { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string AssigneeIdentifier { get; set; }
        [Required]
        public string StatusIdentifier { get; set; }
    }
}
