using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Task
{
    public class AddTaskRequest
    {
        public string? ParentTaskIdentifier { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string DepartmentIdentifier { get; set; }
        [Required]
        public string AssigneeIdentifier { get; set; }
        [Required]
        public string StatusIdentifier { get; set;}
    }
}
