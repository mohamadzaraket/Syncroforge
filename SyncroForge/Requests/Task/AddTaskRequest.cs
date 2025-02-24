using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Requests.Task
{
    public class AddTaskRequest
    {
        public int? ParentTaskId { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        [Required]
        public int AssigneeId { get; set; }
    }
}
