using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Task
{
    public class UpdateTaskRequest
    {
        [Required]
        public string TaskIdentifier { get; set; }
        public string? ParentTaskIdentifier { get; set; }
        public string? Summary { get; set; }
        public string? Description { get; set; }
        public string? AssigneeIdentifier { get; set; }
        public string? StatusIdentifier { get; set; }
    }
}
