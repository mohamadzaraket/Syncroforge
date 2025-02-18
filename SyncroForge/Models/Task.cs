using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SyncroForge.Models
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PublicKey { get; set; }
        public int ParentTaskId { get; set; }
        public virtual Task ParentTask { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public int AssigneeId { get; set; }
        public virtual Employee Assignee { get; set; }
        public int CreatedById { get; set; }
        public virtual Employee Creator { get; set; }
        public int StatusId { get; set; }
        public virtual Status Status { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
       public virtual ICollection<Task> SubTasks {  get; set; }
        public virtual ICollection<TaskHistory> Histories { get; set; }

        public Task()
        {
            PublicKey = Guid.NewGuid().ToString();
            IsDeleted = false;
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            UpdatedAt = CreatedAt;
        }
    }
}
